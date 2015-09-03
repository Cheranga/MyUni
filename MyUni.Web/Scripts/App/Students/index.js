var app = app || {};

app.StudentsViewModel = function(options) {
    var self = {};

    self.searchUrl = options.searchUrl || '';
    self.editUrl = options.editUrl || '';
    self.detailsUrl = options.detailsUrl || '';
    self.deleteUrl = options.deleteUrl || '';

    //
    // Initialize the grid
    //
    self.studentsView = $("#tblStudentsView").DataTable({
        columns: [
            { data: "FirstName" },
            { data: "LastName" },
            {
                data: "EnrolledDate",
                render: function(dateObj) {
                    //
                    // To format the date properly
                    //  http://stackoverflow.com/questions/726334/asp-net-mvc-jsonresult-date-format
                    //
                    var oDate = new Date(parseInt(dateObj.substr(6))); //new Date(dateObj.aData[0]);
                    var result = oDate.getDate() + "/" + (oDate.getMonth() + 1) + "/" + oDate.getFullYear();
                    return "<span>" + result + "</span>";
                }
            },
            {
                data: null,
                sortable: false,
                render: function() {
                    return $('#tmpActions').html();
                }
            }
        ],
        "paging": true,
        "lengthChange": false,
        "searching": false,
        "autoWidth": true,
        //"searchable":false,
        "processing": true,
        "serverSide": true,
        "ajax": self.searchUrl
    });

    //
    // Search function
    //
    self.search = function() {
        var url = self.searchUrl + "?search=" + $('#search').val();
        self.studentsView.ajax.url(url).load();
    };

    //
    // Initialize the edit buttons
    //
    self.initCommandButtons = function () {

        //
        // Upon selection the grid row will be marked as selected
        //
        $('#tblStudentsView tbody').on('click', 'tr', function () {

            $('#tblStudentsView tbody tr').removeClass('warning');
            $(this).addClass('warning');
        });

        //
        // Edit button
        //
        $('#tblStudentsView').on('click', 'tbody tr .editBtn', function () {

            var tr = $(this).closest('tr');
            var data = self.studentsView.row(tr).data();

            var editUrl = self.editUrl + '?id=' + data.Id;
            window.location.href = editUrl;
        });
        //
        // Details button
        //
        $('#tblStudentsView').on('click', 'tbody tr .detailsBtn', function () {

            var tr = $(this).closest('tr');
            var data = self.studentsView.row(tr).data();

            var detailsUrl = self.detailsUrl + '?id=' + data.Id;
            window.location.href = detailsUrl;
        });

        //
        // Delete button
        //
        $('#tblStudentsView').on('click', 'tbody tr .deleteBtn', function () {

            var tr = $(this).closest('tr');
            var data = self.studentsView.row(tr).data();

            var deleteUrl = self.deleteUrl + '?id=' + data.Id;
            $('#deleteUrl').val(deleteUrl);
            $('#confirmDelete').modal('show');
        });

        $('#btnYes').on('click', function(e) {
            $.ajax({
                url: $('#deleteUrl').val(),
                method: 'POST',
                dataType: 'json'
            }).done(function(data) {
                //
                // Refresh the data source
                //
                self.studentsView.ajax.reload();
                $('#confirmDelete').modal('hide');
            });
        });

    };

    //
    // Set the search functionality to happen upon ENTER key press, and on the search button click
    //
    self.initSearch = function () {

        $('#search').on('keypress', function (e) {
            var key = e.which;

            if (key === 13) {
                self.search();
            }
        });
    };

    self.init = function() {
        self.initSearch();
        self.initCommandButtons();
    };

    return {
        init: self.init
    };
}