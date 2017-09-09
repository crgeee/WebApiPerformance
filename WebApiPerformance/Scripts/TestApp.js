(function () {

    $(document).ready(function() {
        execute();
    });


    /**
     * @function columnFilterDataSource
     * @desc Builds custom data source for Kendo grid column filters.
     * @author Chris G.
     * @param {string} columnField - Name of database field corresponding to column
     * @returns {kendo.data.DataSource} Data source for column filters
     * @memberOf APS.Factories.PblInstalled
     * @instance
     */
    function columnFilterDataSource(columnField) {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: 'api/values/grid/columnFilter',
                    cache: false,
                    type: 'post',
                    data: { columnFilter: columnField },
                    contentType: 'application/json',
                    dataType: 'json'
                },
                parameterMap: function (options, operation) {
                    if (operation === 'read') {
                        // Get query parameters for building new single query
                        // default parameter overrides
                        options['workspaceFilters'] = {};
                        return kendo.stringify(options);
                    }
                    return options;
                }
            }
        });
    }

    function execute() {
        $("#grid").kendoGrid({
            columns: [
                {
                    title: "Id",
                    field: "Id",
                    encoded: true
                },
                {
                    title: "Value1",
                    field: "Value1",
                    type: 'string',
                    filterable: {
                        multi: true,
                        search: true,
                        dataSource: columnFilterDataSource('Value1')
                    },
                    encoded: true
                },
                {
                    title: "Value2",
                    field: "Value2",
                    encoded: true
                },
                {
                    title: "Value3",
                    field: "Value3",
                    encoded: true
                },
                {
                    title: "Value4",
                    field: "Value4",
                    encoded: true
                },
                {
                    title: "Value5",
                    field: "Value5",
                    encoded: true
                },
                {
                    title: "Value6",
                    field: "Value6",
                    encoded: true
                },
                {
                    title: "Value7",
                    field: "Value7",
                    encoded: true
                },
                {
                    title: "Value8",
                    field: "Value8",
                    encoded: true
                },
                {
                    title: "Value9",
                    field: "Value9",
                    encoded: true
                },
                {
                    title: "Value10",
                    field: "Value10",
                    encoded: true
                },
                {
                    title: "Value11",
                    field: "Value11",
                    encoded: true
                },
                {
                    title: "Value12",
                    field: "Value12",
                    encoded: true
                },
                {
                    title: "Value13",
                    field: "Value13",
                    encoded: true
                },
                {
                    title: "Value14",
                    field: "Value14",
                    encoded: true
                },
                {
                    title: "Value15",
                    field: "Value15",
                    encoded: true
                },
                {
                    title: "Value16",
                    field: "Value16",
                    encoded: true
                },
                {
                    title: "Value17",
                    field: "Value17",
                    encoded: true
                },
                {
                    title: "Value18",
                    field: "Value18",
                    encoded: true
                },
                {
                    title: "Value19",
                    field: "Value19",
                    encoded: true
                },
                {
                    title: "Value20",
                    field: "Value20",
                    encoded: true
                }
            ],
            dataSource: {
                transport: {
                    read: function (e) {
                        var grid = $("#grid").data('kendoGrid');
                        if (grid == null) {
                            alert('queryGrid: grid is null');
                            return;
                        }

                        // Get query parameters for building new single query
                        // default parameter overrides
                        e.data.page = grid.dataSource.page();
                        e.data.filter = grid.dataSource.filter();
                        e.data.sort = grid.dataSource.sort();
                        e.data.pageSize = grid.dataSource.pageSize();
                        e.data.take = grid.dataSource.take();
                        e.data.workspaceFilters = {};

                        var options = kendo.stringify(e.data);
                        $.ajax({
                            type: 'POST',
                            url: '/api/values/grid',
                            contentType: "application/json; charset=utf-8",
                            data: options,
                            dataType: 'json',
                            success: function (data) {
                                e.success(data);
                            },
                            error: function (error) {
                                alert('Error in Grid GET:' + error);
                            }
                        });
                    },
                    parameterMap: function (options, operation) {
                        if (operation === 'read') {
                            // convert the parameters to a json object
                            options['workspaceFilters'] = {};
                            return kendo.stringify(options);
                        }
                        return options;
                    }
                },
                pageSize: 250,
                schema: {
                    data: function (response) {
                        return response['Data'];
                    },
                    type: 'json',
                    total: function (response) {
                        return response['Count'] || response.length || 0;
                    },
                    // map the errors if there are any. this automatically raises the "error"
                    errors: 'Error'
                },
                error: function (e) {
                    var statusText = e.xhr != null ? e.xhr.statusText : '';
                    alert('An error occurred. Response: ' + statusText);
                },
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true
            },
            filterable: true,
            messages: {
                noRecords: "No records available."
            },
            pageable: {
                pageSizes: [50, 100, 250, 1000],
                buttonCount: 5
            },
            resizeable: true,
            sortable: true
        });

    }
})();