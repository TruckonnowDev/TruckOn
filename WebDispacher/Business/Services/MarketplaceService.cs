using AutoMapper;
using DaoModels.DAO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebDispacher.Business.Extensions;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Service;
using WebDispacher.ViewModels.Marketplace;

namespace WebDispacher.Business.Services
{
    public class MarketplaceService : IMarketplaceService
    {
        private readonly Context db;
        private readonly IMapper mapper;
        private readonly ICompanyService companyService;

        public MarketplaceService(
            IMapper mapper,
            Context db,
            ICompanyService companyService)
        {
            this.mapper = mapper;
            this.db = db;
            this.companyService = companyService;
        }

        public async Task<List<BuyItemMarketPostShortViewModel>> GetPublicBuyItemsMarketPosts(int page)
        {
            var buyItems = db.BuyItemsMarketsPosts
                .Include(c => c.PhoneNumber)
                .Include(bimp => bimp.MarketPost)
                .Where(c => c.MarketPost.ConditionPost == ConditionPost.Published)
                .OrderByDescending(c => c.Id)
                .AsQueryable();

            if (page == UserConstants.AllPagesNumber) return await buyItems.ProjectToBuyItemMarketPostShort();

            try
            {
                buyItems = buyItems.Skip(UserConstants.NormalPageCount * page - UserConstants.NormalPageCount);

                buyItems = buyItems.Take(UserConstants.NormalPageCount);
            }
            catch (Exception)
            {
                buyItems = buyItems.Skip((UserConstants.NormalPageCount * page) - UserConstants.NormalPageCount);
            }

            var listBuyItems = await buyItems.ProjectToBuyItemMarketPostShort();

            return listBuyItems;
        }

        public async Task<int> GetPublicBuyItemsCount()
        {
            var countBuyItems = await db.BuyItemsMarketsPosts
                .Include(bimp => bimp.MarketPost.ConditionPost)
                .CountAsync();

            var countPages = GetCountPage(countBuyItems, UserConstants.NormalPageCount);

            return countPages;
        }

        public async Task CreateBuyLot(CreateBuyLotViewModel model, string companyId, string localDate)
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

            await db.MarketPosts.AddAsync(marketPost);
            var photolist = new PhotoListMP();
            await db.PhotosListMPs.AddAsync(photolist);
            await db.SaveChangesAsync();

            var buyItem = new BuyItemMarketPost
            {
                Title = model.Title,
                Description = model.Description,
                Email= model.Email,
                ZipCode = model.ZipCode,
                MarketPostId = marketPost.Id,
                PhotoListMPId = photolist.Id,
            };

            await db.BuyItemsMarketsPosts.AddAsync(buyItem);

            await db.SaveChangesAsync();
        }

        private int GetCountPage(int countElements, int countElementsInOnePage)
        {
            var countPages = (countElements / countElementsInOnePage) % countElementsInOnePage;

            return countPages > 0 ? countPages + 1 : countPages;
        }
    }
}
