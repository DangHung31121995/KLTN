﻿var BillController = function () {
    var cachedObj = {
        products: [],
        colors: [],
        sizes: [],
        paymentMethods: [],
        billStatuses: []
    }
    this.initialize = function () {
        $.when(loadBillStatus(),
            loadPaymentMethod(),
            loadColors(),
            loadSizes(),
            loadProducts())
            .done(function () {
                loadData();
            });

        registerEvents();
    }

    function registerEvents() {
        $('#txtFromDate, #txtToDate').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy'
        });
        //Init validation
        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            lang: 'vi',
            rules: {
                txtCustomerName: { required: true },
                txtCustomerAddress: { required: true },
                txtCustomerMobile: { required: true },
                txtCustomerMessage: { required: true },
                ddlBillStatus: { required: true }
            }
        });
        $('#txt-search-keyword').keypress(function (e) {
            if (e.which === 13) {
                e.preventDefault();
                loadData();
            }
        });
        $("#btn-search").on('click', function () {
            loadData();
        });

        $("#btn-create").on('click', function () {
            resetFormMaintainance();
            $('#modal-detail').modal('show');
        });
        $("#ddl-show-page").on('change', function () {
            shoponline.configs.pageSize = $(this).val();
            shoponline.configs.pageIndex = 1;
            loadData(true);
        });

        $('body').on('click', '.btn-view', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            $.ajax({
                type: "GET",
                url: "/Admin/Bill/GetById",
                data: { id: that },
                contentType:'json',
                success: function (response) {
                    var data = response;
                    $('#hidId').val(data.id);
                    $('#txtCustomerName').val(data.customerName);

                    $('#txtCustomerAddress').val(data.customerAddress);
                    $('#txtCustomerMobile').val(data.customerMobile);
                    $('#txtCustomerMessage').val(data.customerMessage);
                    $('#ddlPaymentMethod').val(data.paymentMethod);
                    $('#ddlCustomerId').val(data.customerId);
                    $('#ddlBillStatus').val(data.billStatus);

                    var billDetails = data.billDetails;
                    debugger;
                    if (data.BillDetails !== null && data.billDetails.length > 0) {
                        var render = '';
                        var templateDetails = $('#template-table-bill-details').html();

                        $.each(billDetails, function (i, item) {
                            var products = getProductOptions(item.productId);
                            var colors = getColorOptions(item.colorId);
                            var sizes = getSizeOptions(item.sizeId);

                            render += Mustache.render(templateDetails,
                                {
                                    Id: item.Id,
                                    Products: products,
                                    Colors: colors,
                                    Sizes: sizes,
                                    Quantity: item.Quantity
                                });
                        });
                        $('#tbl-bill-details').html(render);
                    }
                    $('#modal-detail').modal('show');
                    shoponline.stopLoading();

                },
                error: () => {

                }
              
            });
        });

        //$('body').on('click', '.btn-view', function (e)
        //{
           
        //        e.preventDefault();
        //    var that = $(this).data('id');
            
        //        debugger;
        //        $.ajax({
        //            type: "GET",
        //            url: "/Admin/Bill/GetById",
        //            data: { id: that },
        //            success: (reponse) => {
        //                alert("ok");
        //            },
        //            error: (e) => {
        //                console.log(e);
        //                debugger;
        //            }
        //        });

        //    });

        $('#btnSave').on('click', function (e) {
            if ($('#frmMaintainance').valid()) {
                e.preventDefault();
                var id = $('#hidId').val();
                var customerName = $('#txtCustomerName').val();
                var customerAddress = $('#txtCustomerAddress').val();
                var customerId = $('#ddlCustomerId').val();
                var customerMobile = $('#txtCustomerMobile').val();
                var customerMessage = $('#txtCustomerMessage').val();
                var paymentMethod = $('#ddlPaymentMethod').val();
                var billStatus = $('#ddlBillStatus').val();
                //bill detail

                var billDetails = [];
                $.each($('#tbl-bill-details tr'), function (i, item) {
                    
                    billDetails.push({
                        Id: $(item).data('id'),
                        ProductId: $(item).find('select.ddlProductId').first().val(),
                        Quantity: $(item).find('input.txtQuantity').first().val(),
                        ColorId: $(item).find('select.ddlColorId').first().val(),
                        SizeId: $(item).find('select.ddlSizeId').first().val(),
                        BillId: id
                    });
                });

                $.ajax({
                    type: "POST",
                    url: "/Admin/Bill/SaveEntity",
                    data: {
                        Id: id,
                        BillStatus: billStatus,
                        CustomerAddress: customerAddress,
                        CustomerId: customerId,
                        CustomerMessage: customerMessage,
                        CustomerMobile: customerMobile,
                        CustomerName: customerName,
                        PaymentMethod: paymentMethod,
                        Status: 1,
                        BillDetails: billDetails
                    },
                    dataType: "json",
                    beforeSend: function () {
                        shoponline.startLoading();
                    },
                    success: function (response) {
                        shoponline.notify('Save order successful', 'success');
                        $('#modal-detail').modal('hide');
                        resetFormMaintainance();

                        shoponline.stopLoading();
                        loadData(true);
                    },
                    error: function () {
                        shoponline.notify('Has an error in progress', 'error');
                        shoponline.stopLoading();
                    }
                });
                return false;
            }

        });

        $('#btnAddDetail').on('click', function () {
            var template = $('#template-table-bill-details').html();
            var products = getProductOptions(null);
            var colors = getColorOptions(null);
            var sizes = getSizeOptions(null);
            var render = Mustache.render(template,
                {
                    Id: 0,
                    Products: products,
                    Colors: colors,
                    Sizes: sizes,
                    Quantity: 0,
                    Total: 0
                });
            $('#tbl-bill-details').append(render);
        });

        $('body').on('click', '.btn-delete-detail', function () {
            $(this).parent().parent().remove();
        });

        $("#btnExport").on('click', function () {
            var that = $('#hidId').val();
            $.ajax({
                type: "POST",
                url: "/Admin/Bill/ExportExcel",
                data: { billId: that },
                beforeSend: function () {
                    shoponline.startLoading();
                },
                success: function (response) {
                    window.location.href = response;
                    shoponline.stopLoading();

                }
            });
        });
    };

    function loadBillStatus() {
        return $.ajax({
            type: "GET",
            url: "/admin/bill/GetBillStatus",
            dataType: "json",
            success: function (response) {
                cachedObj.billStatuses = response;
                var render = "";
                $.each(response, function (i, item) {
                    render += "<option value='" + item.value + "'>" + item.name + "</option>";
                });
                $('#ddlBillStatus').html(render);
            }
        });
    }

    function loadPaymentMethod() {
        return $.ajax({
            type: "GET",
            url: "/admin/bill/GetPaymentMethod",
            dataType: "json",
            success: function (response) {
                cachedObj.paymentMethods = response;
                var render = "";
                $.each(response, function (i, item) {
                    render += "<option value='" + item.value + "'>" + item.name + "</option>";
                });
                $('#ddlPaymentMethod').html(render);
            }
        });
    }

    function loadProducts() {
        return $.ajax({
            type: "GET",
            url: "/Admin/Product/GetAll",
            dataType: "json",
            success: function (response) {
                cachedObj.products = response;
            },
            error: function () {
                shoponline.notify('Has an error in progress', 'error');
            }
        });
    }

    function loadColors() {
        return $.ajax({
            type: "GET",
            url: "/Admin/Bill/GetColors",
            dataType: "json",
            success: function (response) {
                cachedObj.colors = response;
            },
            error: function () {
                shoponline.notify('Has an error in progress', 'error');
            }
        });
    }

    function loadSizes() {
        return $.ajax({
            type: "GET",
            url: "/Admin/Bill/GetSizes",
            dataType: "json",
            success: function (response) {
                cachedObj.sizes = response;
            },
            error: function () {
                shoponline.notify('Has an error in progress', 'error');
            }
        });
    }

    function getProductOptions(selectedId) {
        var products = "<select class='form-control ddlProductId'>";
        $.each(cachedObj.products, function (i, product) {
            if (selectedId === product.id)
                products += '<option value="' + product.id + '" selected="select">' + product.name + '</option>';
            else
                products += '<option value="' + product.id + '">' + product.name + '</option>';
        });
        products += "</select>";
        return products;
    }

    function getColorOptions(selectedId) {
        var colors = "<select class='form-control ddlColorId'>";
        $.each(cachedObj.colors, function (i, color) {
            if (selectedId === color.Id)
                colors += '<option value="' + color.id + '" selected="select">' + color.name + '</option>';
            else
                colors += '<option value="' + color.id + '">' + color.name + '</option>';
        });
        colors += "</select>";
        return colors;
    }

    function getSizeOptions(selectedId) {
        var sizes = "<select class='form-control ddlSizeId'>";
        $.each(cachedObj.sizes, function (i, size) {
            if (selectedId === size.Id)
                sizes += '<option value="' + size.id + '" selected="select">' + size.name + '</option>';
            else
                sizes += '<option value="' + size.id + '">' + size.name + '</option>';
        });
        sizes += "</select>";
        return sizes;
    }
    function resetFormMaintainance() {
        $('#hidId').val(0);
        $('#txtCustomerName').val('');

        $('#txtCustomerAddress').val('');
        $('#txtCustomerMobile').val('');
        $('#txtCustomerMessage').val('');
        $('#ddlPaymentMethod').val('');
        $('#ddlCustomerId').val('');
        $('#ddlBillStatus').val('');
        $('#tbl-bill-details').html('');
    }

    function loadData(isPageChanged) {
        $.ajax({
            type: "POST",
            url: "/admin/bill/GetAllPaging",
            data: {
                startDate: $('#txtFromDate').val(),
                endDate: $('#txtToDate').val(),
                searchText: $('#txtSearchKeyword').val(),
                page: shoponline.configs.pageIndex,
                pageSize: shoponline.configs.pageSize
            },
            dataType: "json",
            beforeSend: function () {
                shoponline.startLoading();
            },
            success: function (response) {
                var template = $('#table-template').html();
                var render = "";
                if (response.result.data.rowCount > 0) {
                    $.each(response.result.data.items, function (i, item) {
                        render += Mustache.render(template, {
                            CustomerName: item.customerName,
                            Id: item.id,
                            PaymentMethod: getPaymentMethodName(item.paymentMethod),
                            DateCreated: shoponline.dateTimeFormatJson(item.dateCreated),
                            BillStatus: getBillStatusName(item.billStatus)
                        });
                    });
                    $("#lbl-total-records").text(response.rowCount);
                    if (render !== undefined) {
                        $('#tbl-content').html(render);

                    }
                    wrapPaging(response.result.data.rowCount, function () {
                        loadData();
                    }, isPageChanged);
                }
                else {
                    $("#lbl-total-records").text('0');
                    $('#tbl-content').html('');
                }
                shoponline.stopLoading();
            },
            error: function (status) {
                console.log(status);
            }
        });
    };
    function getPaymentMethodName(paymentMethod) {
        var method = $.grep(cachedObj.paymentMethods, function (element, index) {
            return element.Value === paymentMethod;
        });
        if (method.length > 0)
            return method[0].Name;
        else return '';
    }
    function getBillStatusName(status) {
        var statuss = $.grep(cachedObj.billStatuses, function (element, index) {
            return element.Value === status;
        });
        if (statuss.length > 0)
            return status[0].Name;
        else return '';
    }
    function wrapPaging(recordCount, callBack, changePageSize) {
        var totalsize = Math.ceil(recordCount / shoponline.configs.pageSize);
        //Unbind pagination if it existed or click change pagesize
        if ($('#paginationUL a').length === 0 || changePageSize === true) {
            $('#paginationUL').empty();
            $('#paginationUL').removeData("twbs-pagination");
            $('#paginationUL').unbind("page");
        }
        //Bind Pagination Event
        $('#paginationUL').twbsPagination({
            totalPages: totalsize,
            visiblePages: 7,
            first: 'Đầu',
            prev: 'Trước',
            next: 'Tiếp',
            last: 'Cuối',
            onPageClick: function (event, p) {
                shoponline.configs.pageIndex = p;
                setTimeout(callBack(), 200);
            }
        });
    }
}