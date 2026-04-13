var table;

function loadGuestTable() {
    table = $('#guestTable').DataTable({
        destroy: true,
        processing: true,
        serverSide: false,
        searching: true,
        responsive: true,
        scrollX: true,
        order: [[0, 'asc']],
        lengthMenu: [10, 25, 50, 100],
        pageLength: 10,

        ajax: {
            url: '/Admin/GetTable',
            type: 'POST',
            dataSrc: 'data'
        },

        columns: [
            { data: 'no', width: '10%' },
            { data: 'name', width: '70%' },
            {
                data: null,
                width: '20%',
                orderable: false,
                render: function (data, type, row) {
                    return `
                        <button class="btn btn-sm btn-danger delete-guest"
                                data-id="${row.Id}">
                            Delete
                        </button>
                    `;
                }
            }
        ],

        language: {
            emptyTable: 'No guest found',
            zeroRecords: 'No matching guest found'
        }
    });
}