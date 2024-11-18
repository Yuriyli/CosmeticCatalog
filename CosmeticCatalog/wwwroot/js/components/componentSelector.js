$(function () {
    //Клик по неактивному компоненту в списке
    $('body').on('click', '.comp-selector', function () {
        var id = $(this).data('id');

        // Добавить выбраннй компонент в контейнеры
        $('#inputComponentContainer').append('<input name="ComponentIds" value="' + id + '" />');
        $('#componentContainer').append('<div class="comp-wrapper" data-id="' + id + '"><p class="comp-in-container" data-id="' + id + '">' + $(this).text() + '</p></div>');

        //Добавить разделитель к предыдущему компоненту, если он есть
        var wrapper = $('.comp-wrapper + [data-id="' + id + '"]');
        var prevElement = wrapper.prev();
        if (prevElement.hasClass('comp-wrapper')) {
            prevElement.append('<p class="comp-devider me-1">,</p>');
        }

        // Показать заголовок, если он скрыт
        if ($('#componentContainerParent').css('display') == 'none') {
            $('#componentContainerParent').show(400);
        }

        // Поменять класс компонента на активный
        $(this).attr('class', 'comp-selector-selected');
    });
    //Клик по активному компоненту в списке
    $('body').on('click', '.comp-selector-selected', function () {
        var id = $(this).data('id');

        // Удалить выбранный компонент из контейнеров
        $('#inputComponentContainer > [value="' + id + '"]').remove();
        var wrapper = $('#componentContainer > [data-id="' + id + '"]');

        // Удалить разделитель из предыдущего
        var prevElement = wrapper.prev();
        var nextElement = wrapper.next();
        if (prevElement.hasClass('comp-wrapper') && !nextElement.hasClass('comp-wrapper')) {
            prevElement.find('.comp-devider').remove();
        }
        wrapper.remove();

        // Убрать заголовок в контейнере, если нет компонентов
        if ($('#componentContainer > .comp-wrapper').length == 0) {
            $('#componentContainerParent').hide(400);
        }

        // Поменять класс компонента на активный
        $(this).attr('class', 'comp-selector');
    });
    // Клик по выбранному компоненту в контейнере
    $('body').on('click', '.comp-in-container', function () {
        var id = $(this).data('id');

        //Поменять класс компонента из списка на неактивный
        $('.comp-selector-selected[data-id="' + id + '"]').attr('class', 'comp-selector');

        //Удалить компонент из контейнеров
        $('#inputComponentContainer > [value="' + id + '"]').remove();
        var wrapper = $('#componentContainer > [data-id="' + id + '"]');

        // Удалить разделитель из предыдущего
        var prevElement = wrapper.prev();
        var nextElement = wrapper.next();
        if (prevElement.hasClass('comp-wrapper') && !nextElement.hasClass('comp-wrapper')) {
            prevElement.find('.comp-devider').remove();
        }
        wrapper.remove();

        // Убрать заголовок в контейнере, если нет компонентов
        if ($('#componentContainer > .comp-wrapper').length == 0) {
            $('#componentContainerParent').hide(400);
        }
    });
});