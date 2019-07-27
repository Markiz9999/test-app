var currentPage = 1;
var maxPageNumber = 1;

var sortBy = '';
var desc = false;
var searchString = '';

$(document).ready(function () {
    searchString = $('#searchInput').val();
    getPage(currentPage);
});

$('.select-all').click(function () {
    $('input[type="checkbox"]').prop('checked', true);

    checkButtonsVisibility();
});

$('.delete-selected').click(function () {
    let checkboxes = $('tbody input[type="checkbox"]:checked');
    let ids = [];

    for (let i = 0; i < checkboxes.length; i++) {
        ids.push(getDataProperty(checkboxes[i], "id"));
    }

    if (ids.length > 0 && confirm("Do you want to delete the selected customers?")) {
        $.ajax(`/Customer/DeleteRange`, {
            method: 'POST',
            data: JSON.stringify(ids),
            contentType: 'application/json; charset=utf-8',
            success: function (data, status, xhr) {
                getPage(currentPage);
            },
            error: function (jqXhr, textStatus, errorMessage) {
                console.error('Error: ' + errorMessage);
            }
        });

        getPage(currentPage);
    }
});

$('#headerCheckbox').click(function () {
    $('input[type="checkbox"]').prop('checked', this.checked);

    checkButtonsVisibility();
});

$('tbody').on('click', 'input[type="checkbox"]', function () {
    checkButtonsVisibility();
});

$('tbody').on('click', '.edit-control', function () {
    let id = getDataProperty(this, "id");
    $(location).attr('href', `/Customer/Edit/${id}`);
});

$('tbody').on('click', '.delete-control', function () {
    let id = getDataProperty(this, "id");
    let login = getDataProperty(this, "login");
    if (id && confirm("Do you want to delete the client with the login '" + login + "'?")) {
        $.ajax(`/Customer/Delete/${id}`, {
            method: 'POST',
            success: function (data, status, xhr) {
                getPage(currentPage);
            },
            error: function (jqXhr, textStatus, errorMessage) {
                console.error('Error: ' + errorMessage);
            }
        });
    }
});

$('.page-buttons').on('click', 'button', function () {
    getPage(parseInt(this.innerText));
});

$('button.first').click(function () {
    getPage(currentPage - 1);
});

$('button.last').click(function () {
    getPage(currentPage + 1);
});

$('#createCustomerButton').click(function () {
    $(location).attr('href', '/Customer/Create');
});

$('#searchForm').submit(function () {
    searchString = $('#searchInput').val();
    getPage(1);

    return false;
});

$('.sort-header').click(function () {
    if (sortBy != this.dataset.prop) {
        sortBy = this.dataset.prop;
    } else if (!desc) {
        desc = !desc;
    } else {
        sortBy = '';
        desc = false;
    }

    getPage(currentPage);
});

function checkButtonsVisibility() {
    let checkedLength = $('tbody input[type="checkbox"]:checked').length
    let isAllSelected = $('tbody input[type="checkbox"]').length === checkedLength;

    $('#headerCheckbox').prop('checked', isAllSelected);

    if (checkedLength > 0) {
        $('.delete-selected').removeClass('hidden');
    } else {
        $('.delete-selected').addClass('hidden');
    }
}

function getPage(pageNumber) {
    let formData = new FormData();
    formData.append("SortBy", sortBy);
    formData.append("Desc", desc);
    formData.append("SearchString", searchString);

    $.ajax(`/Customer/GetPage/${pageNumber}`, {
        method: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (data, status, xhr) {
            $('#totalItems').html(data.ItemsCount);
            updateCustomers(data.Customers);
            updatePagination(data.PageNumber, Math.ceil(data.ItemsCount / data.PageSize));
            checkButtonsVisibility();
        },
        error: function (jqXhr, textStatus, errorMessage) {
            console.error('Error: ' + errorMessage);
        }
    });
}

function updateCustomers(customers) {
    $("tbody").html('');

    for (let i = 0; i < customers.length; i++) {
        let activeColumnStyle = customers[i].Active ? "check-circle" : "minus-circle";

        $("tbody").append(`
            <tr data-id="${customers[i].CustomerId}" data-login="${customers[i].Login}">
                <td><input type="checkbox"/></td>
                <td><span class="edit-control">${customers[i].Login}</span></td>
                <td><span class="edit-control">${customers[i].FirstName} ${customers[i].LastName}</span></td>
                <td><a href="mailto:${customers[i].Email}">${customers[i].Email}</a></td>
                <td>${customers[i].Phone}</td>
                <td class="center">
                    <i class="fa fa-${activeColumnStyle}"></i>
                </td>
                <td class="center">
                    <i class="fa fa-pen edit-control"></i>
                    <i class="fa fa-times-circle delete-control"></i>
                </td>
            </tr>
        `);
    }
}

function updatePagination(page, maxPage) {
    currentPage = page;
    maxPageNumber = maxPage;

    $('.first').attr('disabled', currentPage <= 1);
    $('.last').attr('disabled', currentPage >= maxPageNumber);

    let pagesToShow = [];

    if (currentPage - 1 > 2) {
        pagesToShow.push(1);
    }

    let startCenterPage = Math.max(Math.min(Math.max(1, currentPage - 2), maxPageNumber - 4), 1);
    let endCenterPage = Math.min(Math.max(Math.min(currentPage + 2, maxPageNumber), 5), maxPageNumber);
    for (let i = startCenterPage; i <= endCenterPage; i++) {
        pagesToShow.push(i);
    }

    if (maxPageNumber - currentPage > 2) {
        pagesToShow.push(maxPageNumber);
    }

    $('.page-buttons').html('');

    for (let i = 0; i < pagesToShow.length; i++) {
        if (currentPage === pagesToShow[i]) {
            $('.page-buttons').append(`<button class="active" disabled>${pagesToShow[i]}</button>`);
        } else {
            $('.page-buttons').append(`<button>${pagesToShow[i]}</button>`);
        }

        if (i + 1 < pagesToShow.length) {
            let spaceNeeded = (pagesToShow[i + 1] - pagesToShow[i] > 1);
            if (spaceNeeded) {
                $('.page-buttons').append(`<span>...</span>`);
            }
        }
    }
}

function getDataProperty(element, property) {
    let value = element.dataset[property];
    while (!value && element.parentElement) {
        element = element.parentElement;
        value = element.dataset[property];
    }

    return value;
}
