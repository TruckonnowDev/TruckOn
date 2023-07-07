const setReplacer = (target, expression) => {
    target.addEventListener('input', () => {
        const parsedValue = target.value.replace(expression, '');

        if (parsedValue !== target.value) {
            target.value = parsedValue;
        }
    });
};