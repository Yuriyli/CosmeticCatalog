//TODO: Переписать, закомментить

$(function () {
    $('.tag-sm').each(function () {
        $(this).on('click', function () {
            var id = $(this).data('id');
            if ($(this).hasClass('tag-sm-selected')) {
                removeFromTagContainer($('#tagContainer > [data-id="' + id + '"]'));
                $('#inputTagContainer > [value="' + id + '"]').remove();
            }
            else {
                if ($('#tagContainer > .tag-wrapper').length == 0) {
                    $('#tagContainer').append('<div class="tag-wrapper" data-id="' + id + '"><p class="tag-s" data-id="' + id + '">'
                        + $(this).text() + '</p></div>');
                }
                else {
                    $('#tagContainer').append('<div class="tag-wrapper" data-id="' + id + '"><p class= "tag-s" data-id="' + id + '">'
                        + $(this).text() + '</p></div>');
                    $('#tagContainer > [data-id="' + id + '"]').prev().append('<p class="tag-devider me-1">,</p>');
                }
                $('#inputTagContainer').append('<input name="TagIds" value="' + id + '"/>')
            }
            $(this).toggleClass('tag-sm-selected');
        });
    });

    $('body').on('click', '.tag-s', function () {
        var thisElem = $(this);
        var id = thisElem.data('id');
        var elemModal = $('#mBody>.tag-wrapper>[data-id="' + id + '"]');
        elemModal.toggleClass('tag-sm-selected');
        removeFromTagContainer($('#tagContainer > [data-id="' + id + '"]'));
        $('#inputTagContainer > [value="' + id + '"]').remove();
    });

});

function removeFromTagContainer(wrap) {
    if (wrap.next().hasClass('tag-wrapper')) {
        wrap.remove();
    }
    else {
        var prev = wrap.prev();
        if (prev.hasClass('tag-wrapper')) {
            prev.find('.tag-devider').remove()
            wrap.remove();
        }
        else {
            wrap.remove();
        }
    }
}