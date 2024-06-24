$(() => {
//    LoadPostData();

    var connection = new signalR.HubConnectionBuilder().withUrl("/signalrServer").build();
    connection.start();

    connection.on("LoadPosts", function () {
//        LoadPostData();
        location.href = '/Posts';
    })

//    LoadPostData();

//    function LoadPostData() {
//        var tr = '';
//
//        $.ajax({
//            url: 'Posts/GetPosts',
//            method: 'GET',
//            success: (result) => {
//                $.each(result, (k, v) => {
//                    tr += `<tr>
//                        <td> ${v.createdDate} </td>
//                        <td> ${v.updatedDate} </td>
//                        <td> ${v.title} </td>
//                        <td> ${v.content} </td>
//                        <td> ${v.publishStatus} </td>
//                        <td> ${v.author.fullname} </td>
//                        <td> ${v.category.categoryName} </td>
//                        <td>
//                            <a href="../Posts/Edit?id=${v.postId}">Edit</a> |
//                            <a href="../Posts/Details?id=${v.postId}">Details</a> |
//                            <a href="../Posts/Delete?id=${v.postId}">Delete</a>
//                        </td>
//                     </tr>`
//                })
//
//                $("#tableBody").html(tr);
//            },
//            error: (error) => {
//                console.log(error);
//            }
//        });
//    }
})