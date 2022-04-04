
$(document).ready(function () {
    console.log("geldi geldi ")
    GetTable();
});


function GetTable() {
    $.ajax({
        type: 'POST',
        dataType: "json",
        /*contentType: 'application/json; charset=utf-8',*/
        /*url: 'home/GetSession?browserName=' + browserName + '&browserVersion=' + fullVersion,*/
        url: 'Home/GetCampaignInformation',
        cache: false,
        /*data: data,*/
        success: function (result) {
            console.log("result")
            console.log(result)
            if (result.data) {
                console.log("result.data")
                console.log(result.data)

                var table = $('#tblOutput').DataTable({
                    data: result.data,
                    responsive: true,

                    dom: `<'row'<'col-sm-12'tr>>
			<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>`,

                    sort: false,
                    lengthMenu: [25, 50, 100, 125],

                    pageLength: 25,

                    language: {
                        'lengthMenu': 'Göster _MENU_',
                    },

                    searchDelay: 500,
                    processing: true,
                    serverSide: false,
                    deferLoading: 57,
                    deferRebder: true,
                    sorting: false,
                    columns: [

                        {
                            data: 'Result'
                        }                        
                    ],
                    initComplete: function () {

                        thisTable = this;
                        $("#tblStudents1_processing").css("display", "none");
                    }                    
                });

            }
            else
                alert(result.errorMessage)

        }
    });
}