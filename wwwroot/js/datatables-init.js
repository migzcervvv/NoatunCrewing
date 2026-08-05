// Generic server-side DataTables initializer.
// Usage: initServerSideTable('#usersTable', '/Admin/UserManagement/UsersJson', columns)
function initServerSideTable(selector, ajaxUrl, columns) {
    return $(selector).DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: ajaxUrl,
            data: function (d) {
                // Map DataTables' default param names onto the simple
                // draw/start/length/search query string the controllers expect.
                return {
                    draw: d.draw,
                    start: d.start,
                    length: d.length,
                    search: d.search.value
                };
            }
        },
        columns: columns
    });
}
