using AutoMapper;
using AutoMapper.QueryableExtensions;
using DaoModels.DAO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebDispacher.Business.Extensions;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Service;
using WebDispacher.ViewModels.Marketplace;
using WebDispacher.ViewModels.Order;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebDispacher.Business.Services
{
    public class MarketplaceService : IMarketplaceService
    {
        private readonly Context db;
        private readonly IMapper mapper;
        private readonly ICompanyService companyService;
        private readonly IMemoryCache memoryCache;
        private readonly IHttpContextAccessor httpContextAccessor;

        private int MaxCountFilesInPost = 20;
        private readonly int maxFileLength = 6 * 1024 * 1024;

        public MarketplaceService(
            IMapper mapper,
            Context db,
            ICompanyService companyService,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IHistoryActionService historyActionService)
        {
            this.mapper = mapper;
            this.db = db;
            this.companyService = companyService;
            this.memoryCache = memoryCache;
            this.httpContextAccessor = httpContextAccessor;
            this.historyActionService = historyActionService;
        }

        public async Task<bool> AddViewToMarketPost(int postId, string userId)
        {
            var lastView = await db.ViewsMarketsPosts
                .Where(vmp => vmp.MarketPostId == postId && vmp.UserId == userId)
                .OrderByDescending(vmp => vmp.DateTimeAction)
                .FirstOrDefaultAsync();

            if (lastView != null && DateTime.UtcNow - lastView.DateTimeAction < TimeSpan.FromHours(24))
            {
                return false;
            }

            var viewMarketPost = new ViewMarketPost
            {
                UserId = userId,
                MarketPostId = postId,
                DateTimeAction = DateTime.UtcNow,
                UserAgent = GetUserAgent(),
                IPAddress = GetIPAddress()
            };

            db.ViewsMarketsPosts.Add(viewMarketPost);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<ItemMarketPostShortViewModel>> GetBuyItemsMarketPosts(BuyMarketPostsFiltersViewModel filters, User user)
        {
            var buyItems = GetFilteredBuyItems(filters, user).ToList();

            var buyItemsViewModel = await ConvertBuyItemsToMarketPostViewModelListAsync(buyItems);

            buyItemsViewModel = SortItemsViewModel(buyItemsViewModel, filters.SortType);

            if (filters.Page == UserConstants.AllPagesNumber)
            {
                return buyItemsViewModel;
            }

            var countEntities = buyItemsViewModel.Count();

            (filters.Page, filters.CountPages) = NormalizeItemsPageParameters(filters.Page, filters.CountPages, countEntities);

            return PaginateItemsViewModel(buyItemsViewModel, filters.Page);
        }


        public async Task<List<ItemMarketPostShortViewModel>> GetSellItemsMarketPosts(SellMarketPostsFiltersViewModel filters, User user)
        {
            var sellItems = await GetFilteredSellItems(filters, user).ToListAsync();

            var sellItemsViewModel = await ConvertSellItemsToMarketPostViewModelListAsync(sellItems);

            sellItemsViewModel = SortItemsViewModel(sellItemsViewModel, filters.SortType);

            if (filters.Page == UserConstants.AllPagesNumber)
            {
                return sellItemsViewModel;
            }

            var countEntities = sellItemsViewModel.Count();

            (filters.Page, filters.CountPages) = NormalizeItemsPageParameters(filters.Page, filters.CountPages, countEntities);

            return PaginateItemsViewModel(sellItemsViewModel, filters.Page);
        }

        public async Task<List<ItemMarketPostShortViewModel>> GetUserItemsMarketPosts(UserMarketPostsFiltersViewModel filters, User user)
        {
            var sellItems = await GetFilteredSellItems(
                new SellMarketPostsFiltersViewModel
                {
                    ConditionItem = filters.ConditionItem,
                    ConditionPost = filters.ConditionPost,
                    FirstPrice = filters.FirstPrice,
                    IsOnlyMyPosts = true,
                    KeyWords = filters.KeyWords,
                    LastPrice = filters.LastPrice,
                },
                user)
                .ToListAsync();

            var buyItems = await GetFilteredBuyItems(
                new BuyMarketPostsFiltersViewModel
                {
                    ConditionPost = filters.ConditionPost,
                    IsOnlyMyPosts = true,
                    KeyWords = filters.KeyWords,
                },
                user)
                .ToListAsync();

            var sellItemsViewModel = await ConvertSellItemsToMarketPostViewModelListAsync(sellItems);

            var buyItemsViewModel = await ConvertBuyItemsToMarketPostViewModelListAsync(buyItems);


            var userItems = new List<ItemMarketPostShortViewModel>(buyItemsViewModel);

            userItems.AddRange(sellItemsViewModel);

            userItems = SortItemsViewModel(userItems, filters.SortType);

            if (filters.Page == UserConstants.AllPagesNumber)
            {
                return sellItemsViewModel;
            }

            var countEntities = userItems.Count();

            (filters.Page, filters.CountPages) = NormalizeItemsPageParameters(filters.Page, filters.CountPages, countEntities);

            return PaginateItemsViewModel(userItems, filters.Page);
        }

        public async Task<int> GetCountInCache(int modelId)
        {
            var cacheKey = $"ViewCountMarketPost_{modelId}";

            if (!memoryCache.TryGetValue(cacheKey, out int viewCount))
            {
                viewCount = await db.ViewsMarketsPosts
                    .Where(vmp => vmp.MarketPostId == modelId)
                    .CountAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                memoryCache.Set(cacheKey, viewCount, cacheEntryOptions);
            }

            return viewCount;
        }

        public async Task<int> GetCountPublicBuyMarketPosts()
        {
            var countBuyMarketPosts = await db.BuyItemsMarketsPosts
                .Where(c => c.MarketPost.ConditionPost == ConditionPost.Published)
                .CountAsync();

            return 1;
        }

        public async Task<BuyItemMarketPostViewModel> GetBuyItemMarketPost(int id, string currentUserId)
        {
            var model = await db.BuyItemsMarketsPosts
                .Include(bimp => bimp.MarketPost)
                .ThenInclude(mp => mp.User)
                .Include(bimp => bimp.PhoneNumber)
                .Include(bimp => bimp.PhotoListMP)
                .ThenInclude(plmp => plmp.Photos)
                .FirstOrDefaultAsync(bimp => bimp.MarketPostId == id);

            if (model == null) return null;

            if (model.MarketPost.ConditionPost == ConditionPost.Published
                || model.MarketPost.ConditionPost == ConditionPost.Sold)
            {
                return mapper.Map<BuyItemMarketPostViewModel>(model);
            }

            return IsHavePermissionToEditMarketPost(id, currentUserId) ? mapper.Map<BuyItemMarketPostViewModel>(model) : null;
        }

        public async Task<SellItemMarketPostViewModel> GetSellItemMarketPost(int id, string currentUserId)
        {
            var model = await db.SellItemsMarketsPosts
                .Include(simp => simp.MarketPost)
                .ThenInclude(mp => mp.User)
                .Include(simp => simp.PhoneNumber)
                .Include(simp => simp.PhotoListMP)
                .ThenInclude(simp => simp.Photos)
                .FirstOrDefaultAsync(simp => simp.MarketPostId == id);
            if (model == null) return null;

            if (model.MarketPost.ConditionPost == ConditionPost.Published
                || model.MarketPost.ConditionPost == ConditionPost.Sold)
            {
                return mapper.Map<SellItemMarketPostViewModel>(model);
            }

            return IsHavePermissionToEditMarketPost(id, currentUserId) ? mapper.Map<SellItemMarketPostViewModel>(model) : null;
        }
        
        public async Task<SellItemMarketPostWithHistoryViewModel> GetSellItemMarketPostWithHistory(int id, string currentUserId)
        {
            var model = await db.SellItemsMarketsPosts
                .Include(simp => simp.MarketPost)
                .ThenInclude(mp => mp.User)
                .Include(simp => simp.PhoneNumber)
                .Include(simp => simp.PhotoListMP)
                .ThenInclude(simp => simp.Photos)
                .FirstOrDefaultAsync(simp => simp.MarketPostId == id);
            if (model == null) return null;

            var history = GetMarketPostHistoryActionsById(model.MarketPostId);

            var sellItemMarketPostViewModel = mapper.Map<SellItemMarketPostViewModel>(model);

            if (model.MarketPost.ConditionPost == ConditionPost.Published
                || model.MarketPost.ConditionPost == ConditionPost.Sold)
            {
                return new SellItemMarketPostWithHistoryViewModel
                {
                    SellItemMarketPost = sellItemMarketPostViewModel,
                    Actions = history,
                };
            }

            return IsHavePermissionToEditMarketPost(id, currentUserId) ?
                new SellItemMarketPostWithHistoryViewModel
            {
                SellItemMarketPost = sellItemMarketPostViewModel,
                Actions = history,
            }
            : null;
        }

        public async Task<bool> RemoveUploadedImage(int id)
        {
            var imageById = await db.PhotosMP.FirstOrDefaultAsync(pmp => pmp.Id == id);

            db.PhotosMP.Remove(imageById);

            await db.SaveChangesAsync();

            return imageById != null;
        }

        public async Task<int> CreateBuyLot(CreateBuyLotViewModel model, List<IFormFile> files, string companyId, string localDate)
        {
            var dateTimeCreate = DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var user = await companyService.GetUserByCompanyId(companyId);

            var marketPost = new MarketPost
            {
                UserId = user.Id,
                ConditionPost = ConditionPost.Published,
                ShowView = true,
                ShowComment = true,
                DateTimeCreate = dateTimeCreate,
                DateTimeLastUpdate = dateTimeCreate,
            };

            var photolist = new PhotoListMP();

            await db.MarketPosts.AddAsync(marketPost);
            await db.PhotosListMPs.AddAsync(photolist);

            var phoneNumber = new PhoneNumber
            {
                Number = model.PhoneNumber.Number,
                DialCode = model.PhoneNumber.DialCode,
                Iso2 = model.PhoneNumber.Iso2,
                Name = model.PhoneNumber.Name,
            };

            await db.PhonesNumbers.AddAsync(phoneNumber);

            await db.SaveChangesAsync();

            var buyItem = new BuyItemMarketPost
            {
                Title = model.Title,
                Description = model.Description,
                Email = model.Email,
                ZipCode = model.ZipCode,
                PhoneNumberId = phoneNumber.Id,
                MarketPostId = marketPost.Id,
                PhotoListMPId = photolist.Id,
            };

            await db.BuyItemsMarketsPosts.AddAsync(buyItem);

            await db.SaveChangesAsync();

            if (files != null && files.Count <= MaxCountFilesInPost)
            {
                foreach (var file in files)
                {
                    await SavePhotoToPost(file, UserConstants.MarketplaceBuyEntityTypeResetPassword, photolist.Id, dateTimeCreate);
                }
            }

            return marketPost.Id;
        }

        public async Task<int> CreateSellLot(CreateSellLotViewModel model, List<IFormFile> files, string companyId, string localDate)
        {
            var dateTimeCreate = DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var user = await companyService.GetUserByCompanyId(companyId);

            var marketPost = new MarketPost
            {
                UserId = user.Id,
                ConditionPost = ConditionPost.Published,
                ShowView = true,
                ShowComment = true,
                DateTimeCreate = dateTimeCreate,
                DateTimeLastUpdate = dateTimeCreate,
            };

            var photolist = new PhotoListMP();

            await db.MarketPosts.AddAsync(marketPost);
            await db.PhotosListMPs.AddAsync(photolist);

            var phoneNumber = new PhoneNumber
            {
                Number = model.PhoneNumber.Number,
                DialCode = model.PhoneNumber.DialCode,
                Iso2 = model.PhoneNumber.Iso2,
                Name = model.PhoneNumber.Name,
            };

            await db.PhonesNumbers.AddAsync(phoneNumber);

            await db.SaveChangesAsync();

            var sellItem = new SellItemMarketPost
            {
                Title = model.Title,
                Description = model.Description,
                Email = model.Email,
                ZipCode = model.ZipCode,
                PhoneNumberId = phoneNumber.Id,
                MarketPostId = marketPost.Id,
                PhotoListMPId = photolist.Id,
                ConditionItem = model.ConditionItem,
                Price = model.Price,
            };

            await db.SellItemsMarketsPosts.AddAsync(sellItem);

            await db.SaveChangesAsync();

            if (files != null && files.Count <= MaxCountFilesInPost)
            {
                foreach (var file in files)
                {
                    await SavePhotoToPost(file, UserConstants.MarketplaceSellEntityTypeResetPassword, photolist.Id, dateTimeCreate);
                }
            }

            return marketPost.Id;
        }

        public async Task UpdateMarketPostById(int postId, string localDate, ConditionPost conditionPost)
        {
            var dateTimeLastUpdate = DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var currentMarketPost = await db.MarketPosts.FirstOrDefaultAsync(mp => mp.Id == postId);

            if (currentMarketPost != null
                && currentMarketPost.ConditionPost != ConditionPost.Sold
                || currentMarketPost.ConditionPost != ConditionPost.Deleted)
            {
                currentMarketPost.ConditionPost = conditionPost;
                currentMarketPost.DateTimeLastUpdate = dateTimeLastUpdate;

                await db.SaveChangesAsync();
            }

        }


        public async Task<int> UpdateBuyLot(BuyItemMarketPostViewModel model, string companyId, List<IFormFile> files, string localDate)
        {
            var dateTimeLastUpdate = DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var currentMarketPost = await db.MarketPosts.FirstOrDefaultAsync(mp => mp.Id == model.MarketPostId);

            if (currentMarketPost != null)
            {
                if (currentMarketPost.ConditionPost == ConditionPost.Sold
                || currentMarketPost.ConditionPost == ConditionPost.Deleted)
                {
                    return currentMarketPost.Id;
                }

                currentMarketPost.ConditionPost = model.MarketPost.ConditionPost;
                currentMarketPost.DateTimeLastUpdate = dateTimeLastUpdate;
            }

            var currentPhoneNumber = await db.PhonesNumbers.FirstOrDefaultAsync(p => p.Id == model.PhoneNumberId);

            if (currentPhoneNumber != null)
            {
                currentPhoneNumber.Number = model.PhoneNumber.Number;
                currentPhoneNumber.DialCode = model.PhoneNumber.DialCode;
                currentPhoneNumber.Iso2 = model.PhoneNumber.Iso2;
                currentPhoneNumber.Name = model.PhoneNumber.Name;
            }

            var currentBuyItemMarketPost = await db.BuyItemsMarketsPosts
                .Include(bimp => bimp.PhotoListMP)
                .ThenInclude(plmp => plmp.Photos)
                .FirstOrDefaultAsync(bimp => bimp.Id == model.Id);

            if (currentBuyItemMarketPost != null)
            {
                var editBuyItemMarketPostViewModel = mapper.Map<BuyItemMarketPostViewModel>(currentBuyItemMarketPost);
                var userId = (await companyService.GetUserByCompanyId(companyId)).Id;
                var authorId = currentMarketPost.UserId;

                var actionData = new HistoryActionData
                {
                    UserId = userId,
                    AuthorId = authorId,
                    IPAddress = GetIPAddress(),
                    UserAgent = GetUserAgent(),
                };

                await historyActionService.CompareAndSaveUpdatedFields<HistoryMarketPostAction, BuyItemMarketPostViewModel>(editBuyItemMarketPostViewModel, model, currentMarketPost.Id, actionData, dateTimeLastUpdate);

                currentBuyItemMarketPost.Title = model.Title;
                currentBuyItemMarketPost.Description = model.Description;
                currentBuyItemMarketPost.Email = model.Email;
                currentBuyItemMarketPost.ZipCode = model.ZipCode;
            }

            await db.SaveChangesAsync();

            if (files != null && files.Count <= MaxCountFilesInPost)
            {
                foreach (var file in files)
                {
                    await SavePhotoToPost(file, UserConstants.MarketplaceBuyEntityTypeResetPassword, currentBuyItemMarketPost.PhotoListMPId, dateTimeLastUpdate);
                }
            }

            return currentMarketPost.Id;
        }

        public async Task<BuyItemMarketPostWithHistoryViewModel> GetBuyItemMarketPostWithHistory(int id, string currentUserId)
        {
            using (var dbt = new Context())
            {
                var model = await dbt.BuyItemsMarketsPosts
                    .Include(bimp => bimp.MarketPost)
                    .ThenInclude(mp => mp.User)
                    .Include(bimp => bimp.PhoneNumber)
                    .Include(bimp => bimp.PhotoListMP)
                    .ThenInclude(plmp => plmp.Photos)
                    .FirstOrDefaultAsync(bimp => bimp.MarketPostId == id);

                if (model == null) return null;

                var history = GetMarketPostHistoryActionsById(model.MarketPostId);

                var buyItemMarketPostViewModel = mapper.Map<BuyItemMarketPostViewModel>(model);

                if (model.MarketPost.ConditionPost == ConditionPost.Published
                    || model.MarketPost.ConditionPost == ConditionPost.Sold)
                {
                    return new BuyItemMarketPostWithHistoryViewModel
                    {
                        BuyItemMarketPost = buyItemMarketPostViewModel,
                        Actions = history,
                    };
                }

                return IsHavePermissionToEditMarketPost(id, currentUserId) ?
                    new BuyItemMarketPostWithHistoryViewModel
                    {
                        BuyItemMarketPost = buyItemMarketPostViewModel,
                        Actions = history,
                    }
                : null;
            }
        }


        public HistoryMarketPostActionGroup GetHistoryForDate(DateTime date, int itemId)
        {
            var historyBuyItemActions = db.HistoriesMarketPostsActions
                .Where(hoa => hoa.MarketPostId == itemId && hoa.DateTimeAction.Date == date.Date)
                .OrderByDescending(hoa => hoa.DateTimeAction)
                .ToList();

            var userIds = historyBuyItemActions.Select(hoa => hoa.UserId).Distinct().ToList();

            var companyNames = db.CompanyUsers
                .Where(cu => userIds.Contains(cu.UserId))
                .Select(cu => new
                {
                    UserId = cu.UserId,
                    CompanyName = cu.Company.Name
                })
                .ToDictionary(x => x.UserId, x => x.CompanyName);

            var groupedHistory = new HistoryMarketPostActionGroup
            {
                GroupAction = date,
                Groups = historyBuyItemActions.Select(hoa =>
                {
                    var historyBuyMarketPostActionViewModel = new HistoryMarketPostActionViewModel
                    {
                        Id = hoa.Id,
                        UserId = hoa.UserId,
                        ActionType = hoa.ActionType,
                        ChangedField = hoa.ChangedField,
                        ContentBefore = hoa.ContentBefore,
                        ContentAfter = hoa.ContentAfter,
                        DateTimeAction = hoa.DateTimeAction,
                        UserAgent = hoa.UserAgent,
                        IpAddress = hoa.IpAddress,
                        AuthorName = companyNames.ContainsKey(hoa.UserId) ? companyNames[hoa.UserId] : "Not Found",
                        CurrentUserName = companyNames.ContainsKey(hoa.UserId) ? companyNames[hoa.UserId] : "Not Found",
                        UserAgentInfoViewModel = GetInfoAboutUserAgent(hoa.UserAgent),
                        IPInfoViewModel = GetInfoAboutIpAddress(hoa.IpAddress),
                    };
                    return historyBuyMarketPostActionViewModel;
                }).ToList()
            };

            return groupedHistory;
        }

        public IPInfoViewModel GetInfoAboutIpAddress(string ipAddress)
        {
            try
            {
                using(var reader = new DatabaseReader(PathDbGeo))
                {
                    var response = reader.City(ipAddress);
                    var ipInfoViewModel = new IPInfoViewModel
                    {
                        CountryName = response.Country.Name,
                        MostSpecificSubdivisionName = response.MostSpecificSubdivision.Name,
                        CityName = response.City.Name,
                        PostalCode = response.Postal.Code,
                        PostalCodeСonfidence = response.Postal.Confidence,
                        Latitude = response.Location.Latitude,
                        Longitude = response.Location.Longitude,
                        LocationAccuracyRadius = response.Location.AccuracyRadius
                    };

                    return ipInfoViewModel;
                }

            }
            catch (Exception ex)
            {
                
            }

            return null;
        }
        
        public UserAgentInfoViewModel GetInfoAboutUserAgent(string userAgent)
        {
            try
            {
                var agentInfo = new DeviceDetector(userAgent);
                agentInfo.Parse();

                if (agentInfo.IsBot()) return null;
                var userAgentViewModel = new UserAgentInfoViewModel();

                var clientInfo = agentInfo.GetClient();
                if(clientInfo != null)
                {
                    userAgentViewModel.BrowserName = clientInfo.Match.Name;
                    userAgentViewModel.BrowserVersion = clientInfo.Match.Version;
                }

                var osInfo = agentInfo.GetOs();
                if (osInfo != null)
                {
                    userAgentViewModel.OSName = osInfo.Match.Name;
                    userAgentViewModel.OSVersion = osInfo.Match.Version;
                    userAgentViewModel.OSPlatform = osInfo.Match.Platform;
                }

                userAgentViewModel.DeviceName = agentInfo.GetDeviceName();
                userAgentViewModel.DeviceBrand = agentInfo.GetBrandName();
                userAgentViewModel.DeviceModel = agentInfo.GetModel();
                userAgentViewModel.IsDesktop = agentInfo.IsDesktop();
                userAgentViewModel.IsMobile = agentInfo.IsMobile();

                return userAgentViewModel;
            }
            catch (Exception ex)
            {
                
            }

            return null;
        }

        public List<GroupsHistoriesListShortViewModel> GetMarketPostHistoryActionsById(int id)
        {
            using (var dbt = new Context())
            {
                var historyBuyItemActions = dbt.HistoriesMarketPostsActions
                    .Where(hoa => hoa.MarketPostId == id)
                    .OrderByDescending(hoa => hoa.DateTimeAction)
                    .AsEnumerable()
                    .ToList();

                var groupedHistoryData = historyBuyItemActions
                    .GroupBy(hoa => hoa.DateTimeAction.Date)
                    .Select(group => new GroupsHistoriesListShortViewModel
                    {
                        GroupAction = group.Key,
                        GroupCount = group.Count()
                    })
                    .ToList();

                return groupedHistoryData;
            }
        }

        public List<HistoryMarketPostActionGroup> GetBuyItemHistoryById(int id)
        {
            var historyBuyItemActions = db.HistoriesMarketPostsActions
                .Where(hoa => hoa.MarketPostId == id)
                .OrderByDescending(hoa => hoa.DateTimeAction)
                .AsEnumerable()
                .ToList();

            var userIds = historyBuyItemActions.Select(hoa => hoa.UserId).Distinct().ToList();

            var companyNames = db.CompanyUsers
                .Where(cu => userIds.Contains(cu.UserId))
                .Select(cu => new
                {
                    UserId = cu.UserId,
                    CompanyName = cu.Company.Name
                })
                .ToDictionary(x => x.UserId, x => x.CompanyName);

            var groupedHistory = historyBuyItemActions
                .GroupBy(hoa => hoa.DateTimeAction.Date)
                .Select(group => new HistoryMarketPostActionGroup
                {
                    GroupAction = group.Key,
                    Groups = group.Select(hoa =>
                    {
                        var historyBuyMarketPostActionViewModel = new HistoryMarketPostActionViewModel
                        {
                            Id = hoa.Id,
                            UserId = hoa.UserId,
                            ActionType = hoa.ActionType,
                            ChangedField = hoa.ChangedField,
                            ContentBefore = hoa.ContentBefore,
                            ContentAfter = hoa.ContentAfter,
                            DateTimeAction = hoa.DateTimeAction,
                            UserAgent = hoa.UserAgent,
                            IpAddress = hoa.IpAddress,
                            AuthorName = companyNames.ContainsKey(hoa.UserId) ? companyNames[hoa.UserId] : "Not Found",
                            CurrentUserName = companyNames.ContainsKey(hoa.UserId) ? companyNames[hoa.UserId] : "Not Found"
                        };
                        return historyBuyMarketPostActionViewModel;
                    }).ToList()
                })
                .ToList();

            return groupedHistory;
        }


        public async Task<int> UpdateSellLot(SellItemMarketPostViewModel model, string companyId, List<IFormFile> files, string localDate)
        {
            var dateTimeLastUpdate = DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var currentMarketPost = await db.MarketPosts.FirstOrDefaultAsync(mp => mp.Id == model.MarketPostId);

            if (currentMarketPost != null)
            {
                if (currentMarketPost.ConditionPost == ConditionPost.Sold
                || currentMarketPost.ConditionPost == ConditionPost.Deleted)
                {
                    return currentMarketPost.Id;
                }

                currentMarketPost.ConditionPost = model.MarketPost.ConditionPost;
                currentMarketPost.DateTimeLastUpdate = dateTimeLastUpdate;
            }

            var currentPhoneNumber = await db.PhonesNumbers.FirstOrDefaultAsync(p => p.Id == model.PhoneNumberId);

            if (currentPhoneNumber != null)
            {
                currentPhoneNumber.Number = model.PhoneNumber.Number;
                currentPhoneNumber.DialCode = model.PhoneNumber.DialCode;
                currentPhoneNumber.Iso2 = model.PhoneNumber.Iso2;
                currentPhoneNumber.Name = model.PhoneNumber.Name;
            }

            var currentSellItemMarketPost = await db.SellItemsMarketsPosts
                .Include(bimp => bimp.PhotoListMP)
                .ThenInclude(plmp => plmp.Photos)
                .FirstOrDefaultAsync(bimp => bimp.Id == model.Id);

            if (currentSellItemMarketPost != null)
            {
                var editSellItemMarketPostViewModel = mapper.Map<SellItemMarketPostViewModel>(currentSellItemMarketPost);
                var userId = (await companyService.GetUserByCompanyId(companyId)).Id;
                var authorId = currentMarketPost.UserId;

                var actionData = new HistoryActionData
                {
                    UserId = userId,
                    AuthorId = authorId,
                    IPAddress = GetIPAddress(),
                    UserAgent = GetUserAgent(),
                };

                await historyActionService.CompareAndSaveUpdatedFields<HistoryMarketPostAction, SellItemMarketPostViewModel>(editSellItemMarketPostViewModel, model, currentMarketPost.Id, actionData, dateTimeLastUpdate);

                currentSellItemMarketPost.Title = model.Title;
                currentSellItemMarketPost.Description = model.Description;
                currentSellItemMarketPost.Email = model.Email;
                currentSellItemMarketPost.ZipCode = model.ZipCode;
                currentSellItemMarketPost.ConditionItem = model.ConditionItem;
                currentSellItemMarketPost.Price = model.Price;
            }

            await db.SaveChangesAsync();

            if (files != null && files.Count <= MaxCountFilesInPost)
            {
                foreach (var file in files)
                {
                    await SavePhotoToPost(file, UserConstants.MarketplaceSellEntityTypeResetPassword, currentSellItemMarketPost.PhotoListMPId, dateTimeLastUpdate);
                }
            }

            return currentMarketPost.Id;
        }

        public bool IsHavePermissionToEditMarketPost(int postId, string userId)
        {
            using (var dbt = new Context())
            {
                var isAuthor = dbt.MarketPosts.Any(mp => mp.Id == postId && mp.UserId == userId);

                return isAuthor;
            }
        }

        public async Task<(int, DateTime)> GetCountViewAndDateLastUpdateBuy()
        {
            var lastUpdateDateTime = await GetDateTimeLastUpdateBuy();

            var totalViewForBuy = await GetTotalViewBuy();

            return (totalViewForBuy, lastUpdateDateTime);
        }

        public async Task<DateTime> GetDateTimeLastUpdateBuy()
        {
            var cacheKey = $"GetDateTimeLastUpdateBuy";

            if (!memoryCache.TryGetValue(cacheKey, out DateTime lastUpdateDateTime))
            {
                lastUpdateDateTime = await db.MarketPosts
                    .Where(mp => db.BuyItemsMarketsPosts.Include(bimp => bimp.MarketPost).Any(bimp => bimp.MarketPostId == mp.Id
                        && bimp.MarketPost.ConditionPost == ConditionPost.Published))
                    .Select(mp => mp.DateTimeLastUpdate)
                    .MaxAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                memoryCache.Set(cacheKey, lastUpdateDateTime, cacheEntryOptions);
            }

            return lastUpdateDateTime;
        }

        public async Task<int> GetTotalViewBuy()
        {
            var cacheKey = $"GetTotalViewBuy";

            if (!memoryCache.TryGetValue(cacheKey, out int lastUpdateDateTime))
            {
                lastUpdateDateTime = await db.ViewsMarketsPosts
                    .Where(vmp => db.BuyItemsMarketsPosts.Include(simp => simp.MarketPost)
                            .Any(bimp => bimp.MarketPostId == vmp.MarketPostId
                            && bimp.MarketPost.ConditionPost == ConditionPost.Published))
                    .CountAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                memoryCache.Set(cacheKey, lastUpdateDateTime, cacheEntryOptions);
            }

            return lastUpdateDateTime;
        }

        public async Task<(int, DateTime)> GetCountViewAndDateLastUpdateSell()
        {
            var lastUpdateDateTime = await GetDateTimeLastUpdateSell();

            var totalViewForBuy = await GetTotalViewSell();

            return (totalViewForBuy, lastUpdateDateTime);
        }

        public async Task<DateTime> GetDateTimeLastUpdateSell()
        {
            var cacheKey = $"GetDateTimeLastUpdateSell";

            if (!memoryCache.TryGetValue(cacheKey, out DateTime lastUpdateDateTime))
            {
                lastUpdateDateTime = await db.MarketPosts
                    .Where(mp => db.SellItemsMarketsPosts.Include(simp => simp.MarketPost).Any(simp => simp.MarketPostId == mp.Id
                        && simp.MarketPost.ConditionPost == ConditionPost.Published))
                    .Select(mp => mp.DateTimeLastUpdate)
                    .MaxAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                memoryCache.Set(cacheKey, lastUpdateDateTime, cacheEntryOptions);
            }

            return lastUpdateDateTime;
        }

        public async Task<int> GetTotalViewSell()
        {
            var cacheKey = $"GetTotalViewSell";

            if (!memoryCache.TryGetValue(cacheKey, out int lastUpdateDateTime))
            {
                lastUpdateDateTime = await db.ViewsMarketsPosts
                    .Where(vmp => db.SellItemsMarketsPosts.Include(simp => simp.MarketPost)
                                .Any(simp => simp.MarketPostId == vmp.MarketPostId
                                && simp.MarketPost.ConditionPost == ConditionPost.Published))
                    .CountAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                memoryCache.Set(cacheKey, lastUpdateDateTime, cacheEntryOptions);
            }

            return lastUpdateDateTime;
        }

        public async Task<(int, DateTime)> GetCountViewAndDateLastUpdateByUserId(string userId)
        {
            var lastUpdateDateTime = await GetDateTimeLastUpdateUserId(userId);

            var totalViewForUser = await GetTotalViewUserId(userId);

            return (totalViewForUser, lastUpdateDateTime);
        }

        public async Task<int> GetTotalViewUserId(string userId)
        {
            var cacheKey = $"GetTotalView_" + userId;

            if (!memoryCache.TryGetValue(cacheKey, out int lastUpdateDateTime))
            {
                lastUpdateDateTime = await db.ViewsMarketsPosts
                    .Where(vmp => db.MarketPosts
                        .Any(simp => simp.Id == vmp.MarketPostId
                                     && simp.UserId == userId))
                    .CountAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                memoryCache.Set(cacheKey, lastUpdateDateTime, cacheEntryOptions);
            }

            return lastUpdateDateTime;
        }

        public async Task<DateTime> GetDateTimeLastUpdateUserId(string userId)
        {
            var cacheKey = $"GetDateTimeLastUpdate_" + userId;

            if (!memoryCache.TryGetValue(cacheKey, out DateTime lastUpdateDateTime))
            {
                lastUpdateDateTime = await db.MarketPosts
                    .Where(mp => db.MarketPosts.Any(simp => simp.Id == mp.Id
                                                            && simp.UserId == userId))
                    .Select(mp => mp.DateTimeLastUpdate)
                    .MaxAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                memoryCache.Set(cacheKey, lastUpdateDateTime, cacheEntryOptions);
            }

            return lastUpdateDateTime;
        }

        public async Task<(int, DateTime)> GetCountViewAndDateLastUpdateAll()
        {
            var lastUpdateDateTime = await GetDateTimeLastUpdateAll();

            var totalViewForBuy = await GetTotalViewAll();

            return (totalViewForBuy, lastUpdateDateTime);
        }

        public async Task<DateTime> GetDateTimeLastUpdateAll()
        {
            var cacheKey = $"GetDateTimeLastUpdateAll";

            if (!memoryCache.TryGetValue(cacheKey, out DateTime lastUpdateDateTime))
            {
                lastUpdateDateTime = await db.MarketPosts
                    .Where(mp => db.MarketPosts.Any(simp => simp.Id == mp.Id
                        && simp.ConditionPost == ConditionPost.Published))
                    .Select(mp => mp.DateTimeLastUpdate)
                    .MaxAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                memoryCache.Set(cacheKey, lastUpdateDateTime, cacheEntryOptions);
            }

            return lastUpdateDateTime;
        }

        public async Task<int> GetTotalViewAll()
        {
            var cacheKey = $"GetTotalViewAll";

            if (!memoryCache.TryGetValue(cacheKey, out int lastUpdateDateTime))
            {
                lastUpdateDateTime = await db.ViewsMarketsPosts
                    .Where(vmp => db.MarketPosts
                                .Any(simp => simp.Id == vmp.MarketPostId
                                && simp.ConditionPost == ConditionPost.Published))
                    .CountAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                memoryCache.Set(cacheKey, lastUpdateDateTime, cacheEntryOptions);
            }

            return lastUpdateDateTime;
        }

        public async Task<bool> AddHistoryMarketPostItem(int postId, string userId)
        {
            /* var marketPost = await db.MarketPosts
                 .FirstOrDefaultAsync(mp => mp.Id == postId);

             if (marketPost == null) return false;

             var viewMarketPost = new HistoryMarketPostAction
             {
                 UserId = userId,
                 MarketPostId = postId,
                 DateTimeAction = DateTime.UtcNow,
                 UserAgent = GetUserAgent(),
                 IPAddress = GetIPAddress()
             };

             db.ViewsMarketsPosts.Add(viewMarketPost);
             await db.SaveChangesAsync();*/

            return true;
        }

        public async Task SavePhotoToPost(IFormFile uploadedFile, string postType, int postId, DateTime dateTimeUpload)
        {
            if (uploadedFile.Length > maxFileLength) return;

            var path = $"../Marketplace/{postType}/{postId}/{uploadedFile.FileName}";

            if (!Directory.Exists($"../Marketplace/{postType}"))
            {
                Directory.CreateDirectory($"../Marketplace/{postType}");
            }

            if (!Directory.Exists($"../Marketplace/{postType}/{postId}"))
            {
                Directory.CreateDirectory($"../Marketplace/{postType}/{postId}");
            }

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                uploadedFile.CopyTo(fileStream);
            }

            using var imageStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var image = System.Drawing.Image.FromStream(imageStream);

            await SavePhotoToPostDb(path, image.Width, image.Height, postId, postType, uploadedFile.FileName, dateTimeUpload);
        }

        public int GetCurrentPhotoTypeIdByName(string name)
        {
            var currentPhotoTypeId = db.PhotoTypes.FirstOrDefault(cs => cs.Type == name).Id;

            return currentPhotoTypeId;
        }

        private IQueryable<BuyItemMarketPost> GetFilteredBuyItems(BuyMarketPostsFiltersViewModel filters, User user)
        {
            var buyItems = db.BuyItemsMarketsPosts
                .Include(c => c.PhoneNumber)
                .Include(bimp => bimp.MarketPost)
                .AsQueryable();

            if (filters.ConditionPost != ConditionPost.NotSelected)
            {
                buyItems = buyItems.Where(bimp => bimp.MarketPost.ConditionPost == filters.ConditionPost);
            }

            if (filters.IsOnlyMyPosts)
            {
                buyItems = buyItems.Where(bimp => bimp.MarketPost.UserId == user.Id);
            }

            if (!string.IsNullOrEmpty(filters.KeyWords))
            {
                var keywords = filters.KeyWords.Split(' ');

                buyItems = keywords.Aggregate(buyItems, (current, keyword) =>
                    current.Where(bimp => bimp.Title.Contains(keyword) || bimp.Description.Contains(keyword))
                );
            }

            return buyItems;
        }

        private IQueryable<SellItemMarketPost> GetFilteredSellItems(SellMarketPostsFiltersViewModel filters, User user)
        {
            var sellItems = db.SellItemsMarketsPosts
                .Include(c => c.PhoneNumber)
                .Include(bimp => bimp.MarketPost)
                .AsQueryable();

            if (filters.ConditionPost != ConditionPost.NotSelected)
            {
                sellItems = sellItems.Where(simp => simp.MarketPost.ConditionPost == filters.ConditionPost);
            }

            if (filters.IsOnlyMyPosts)
            {
                sellItems = sellItems.Where(simp => simp.MarketPost.UserId == user.Id);
            }

            if (filters.FirstPrice != 0)
            {
                sellItems = sellItems.Where(simp => simp.Price > filters.FirstPrice);
            }

            if (filters.LastPrice != 0)
            {
                sellItems = sellItems.Where(simp => simp.Price < filters.LastPrice);
            }

            if (!string.IsNullOrEmpty(filters.KeyWords))
            {
                var keywords = filters.KeyWords.Split(' ');

                sellItems = keywords.Aggregate(sellItems, (current, keyword) =>
                    current.Where(bimp => bimp.Title.Contains(keyword) || bimp.Description.Contains(keyword))
                );
            }

            return sellItems;
        }

        private async Task<List<ItemMarketPostShortViewModel>> ConvertBuyItemsToMarketPostViewModelListAsync(List<BuyItemMarketPost> buyItemsList)
        {
            var buyItemsViewModel = new List<ItemMarketPostShortViewModel>();

            foreach (var buyItem in buyItemsList)
            {
                var viewModel = await ConvertToViewModelBuyItemMarketPostAsync(buyItem);
                buyItemsViewModel.Add(viewModel);
            }

            return buyItemsViewModel;
        }

        private async Task<List<ItemMarketPostShortViewModel>> ConvertSellItemsToMarketPostViewModelListAsync(List<SellItemMarketPost> sellItemsList)
        {
            var sellItemsViewModel = new List<ItemMarketPostShortViewModel>();

            foreach (var sellItem in sellItemsList)
            {
                var viewModel = await ConvertToViewModelSellItemMarketPostAsync(sellItem);
                sellItemsViewModel.Add(viewModel);
            }

            return sellItemsViewModel;
        }

        private List<ItemMarketPostShortViewModel> SortItemsViewModel(List<ItemMarketPostShortViewModel> itemsViewModel, SortType sortType)
        {
            switch (sortType)
            {
                case SortType.MinViews:
                    {
                        return itemsViewModel.OrderBy(impsvm => impsvm.ViewCount).ToList();
                    }
                case SortType.MaxViews:
                    {
                        return itemsViewModel.OrderByDescending(impsvm => impsvm.ViewCount).ToList();
                    }
                case SortType.New:
                    {
                        return itemsViewModel.OrderByDescending(impsvm => impsvm.DateTimeCreate).ToList();
                    }
                case SortType.LastUpdate:
                    {
                        return itemsViewModel.OrderByDescending(impsvm => impsvm.DateTimeLastUpdate).ToList();
                    }
                case SortType.MinPrice:
                    {
                        return itemsViewModel.OrderBy(impsvm => impsvm.Price).ToList();
                    }
                case SortType.MaxPrice:
                    {
                        return itemsViewModel.OrderByDescending(impsvm => impsvm.Price).ToList();
                    }
                default:
                    {
                        return itemsViewModel.OrderByDescending(impsvm => impsvm.Id).ToList();
                    }
            }
        }

        private (int Page, int CountPages) NormalizeItemsPageParameters(int page, int countPage, int countEntities)
        {
            if (UserConstants.NormalPageCount > countEntities)
            {
                countPage = 1;
                page = 1;
            }
            else
            {
                countPage = GetCountPage(countEntities, UserConstants.NormalPageCount);

                if (countPage < page)
                {
                    page = 1;
                }
            }

            return (page, countPage);
        }

        private List<ItemMarketPostShortViewModel> PaginateItemsViewModel(List<ItemMarketPostShortViewModel> buyItemsViewModel, int page)
        {
            try
            {
                return buyItemsViewModel
                    .Skip(UserConstants.NormalPageCount * page - UserConstants.NormalPageCount)
                    .Take(UserConstants.NormalPageCount).ToList();
            }
            catch (Exception)
            {
                return buyItemsViewModel
                    .Skip((UserConstants.NormalPageCount * page) - UserConstants.NormalPageCount)
                    .ToList();
            }
        }

        private async Task SavePhotoToPostDb(string path, int width, int height, int postId, string postType, string nameDoc, DateTime localDate)
        {
            var pref = path.Remove(0, path.LastIndexOf(".") + 1);

            var photoMP = new PhotoMP()
            {
                PhotoPath = path,
                Name = nameDoc,
                Width = width,
                Height = height,
                PhotoListMPId = postId,
                PhotoTypeId = GetCurrentPhotoTypeIdByName(postType),
                DateTimeUpload = localDate,
            };

            await db.PhotosMP.AddAsync(photoMP);

            await db.SaveChangesAsync();
        }

        private int GetCountPage(int countElements, int countElementsInOnePage)
        {
            var countPages = (countElements / countElementsInOnePage) % countElementsInOnePage;

            return countPages > 0 ? countPages + 1 : countPages;
        }

        private string GetUserAgent()
        {
            return httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
        }

        private string GetIPAddress()
        {
            return httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        private async Task<ItemMarketPostShortViewModel> ConvertToViewModelBuyItemMarketPostAsync(BuyItemMarketPost model)
        {
            return new ItemMarketPostShortViewModel
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                UserId = model.MarketPost.UserId,
                MarketPostId = model.MarketPostId,
                User = model.MarketPost.User,
                ConditionPost = model.MarketPost.ConditionPost,
                ShowView = model.MarketPost.ShowView,
                ShowComment = model.MarketPost.ShowComment,
                DateTimeLastUpdate = model.MarketPost.DateTimeLastUpdate,
                DateTimeCreate = model.MarketPost.DateTimeCreate,
                ViewCount = await GetCountInCache(model.MarketPostId),
                TypeMarketPost = UserConstants.MarketplaceBuyCategory,
            };
        }

        private async Task<ItemMarketPostShortViewModel> ConvertToViewModelSellItemMarketPostAsync(SellItemMarketPost model)
        {
            return new ItemMarketPostShortViewModel
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                UserId = model.MarketPost.UserId,
                MarketPostId = model.MarketPostId,
                User = model.MarketPost.User,
                ConditionPost = model.MarketPost.ConditionPost,
                ShowView = model.MarketPost.ShowView,
                ShowComment = model.MarketPost.ShowComment,
                DateTimeLastUpdate = model.MarketPost.DateTimeLastUpdate,
                DateTimeCreate = model.MarketPost.DateTimeCreate,
                ConditionItem = model.ConditionItem,
                Price = model.Price,
                ViewCount = await GetCountInCache(model.MarketPostId),
                TypeMarketPost = UserConstants.MarketplaceSellCategory,
            };
        }
    }
}
