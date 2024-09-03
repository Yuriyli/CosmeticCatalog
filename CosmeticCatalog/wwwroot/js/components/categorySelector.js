$(function () {
    //Клик по не активной категории в списке
    $('body').on('click', '.c-selector', function () {
        var id = $(this).data('id');
        // Очистить выбор в контейнерах
        $('#inputContainer').empty();
        $('#categoryContainer').empty();
        // Добавить выбранную категорию в контейнеры, показать заголовок
        $('#inputContainer').append('<input name="parentId" value="' + id + '" />');
        $('#categoryContainer').append('<p class="c-in-container" data-id="' + id + '">' + $(this).text() + '</p>');
        $('#categoryContainerParent').show(400);
        // Переключить на противоположные классы активную и неактивную категорию
        $('.c-selector-selected').attr('class', 'c-selector');
        $(this).attr('class', 'c-selector-selected');
    });

    // Клик по выбранной категории в списке
    $('body').on('click', '.c-selector-selected', function () {
        // Очистить выбор в контейнерах, спрятать заголовок
        $('#inputContainer').empty();
        $('#categoryContainer').empty();
        $('#categoryContainerParent').hide(400);
        // Переключить на противоположный класс
        $(this).attr('class', 'c-selector');
    });

    // Клик по выбранной категории в контейнере
    $('body').on('click', '.c-in-container', function () {
        // Очистить выбор в контейнерах, спрятать заголовок
        $('#inputContainer').empty();
        $('#categoryContainer').empty();
        $('#categoryContainerParent').hide(400);
        // переключить класс в списке на неактивный
        $('.c-selector-selected').attr('class', 'c-selector');
    });
});