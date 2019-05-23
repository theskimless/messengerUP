//CREATE GROUP
var cgGroupType = 0;
$("#cg-group-type-select").on("change", function () {
    var text = "";
    //CHAT
    if (+$(this).val() === 0) {
        text = "чат";
        $("#cg-block-group").slideUp();
        $("#cg-find-user-input").attr("placeholder", "Введите логин или email");
        cgGroupType = 0;
    }
    //GROUP
    else {
        cgGroupType = 1;
        text = "группу";
        $("#cg-block-group").slideDown();
        $("#cg-find-user-input").attr("placeholder", "Введите логин или email через запятую");
    }
    $("#cg-stitle").text(text);
});

$("#cg-find-user-input").on("keyup", function () {
    $(".cg-user-block").slideUp();
    $(".cg-user-block").html("");
    $("#cg-users").val("");

    var name = $(this).val();
    if (name !== "") {
        $.get({
            url: "/Home/FindUser?groupType=" + cgGroupType + "&name=" + name,
            success: function (res) {
                if (res !== "") {
                    var users = JSON.parse(res);
                    var usersInp = "";
                    for (var i = 0; i < users.length; i++) {
                        var user = users[i];
                        var userAvatar = "022-smile.svg";
                        if (user.Avatar !== null) userAvatar = user.Avatar;

                        $(".cg-user-block").append("<div class='d-flex'><div><img width='40' id='c-user-img' src='/db_files/" + userAvatar + "'/></div><div class='cg-user-block-info'><div>" + user.Login + "</div><div>" + user.Email + "</div></div></div>");
                        usersInp += user.Login + ",";
                    }
                    $("#cg-users").val(usersInp);
                    $(".cg-user-block").slideDown();
                }
            }
        });
    }
});


//MANAGE
$(".mprofile-avatar").on("click", function () {
    var name = "";
    if ($(this).hasClass("cg-smile-active")) {
        name = "";
        $(this).removeClass("cg-smile-active");
        $("#cg-input-file").prop("disabled", false);
    }
    else {
        name = $(this).attr("fname");
        $("#cg-input-file").prop("disabled", true);
        $(".cg-smile").removeClass("cg-smile-active");
        $(this).addClass("cg-smile-active");
        $("#cg-input-file").val("");
    }

    $("#def-avatar-inp").val(name);
});

$("#cg-input-file").on("change", function () {
    var file = this.files[0];
    if (file.size > 4096 * 1024) {
        alert("Размер картинки не может превышать 4 мб.");
        $(this).val("");
    }
    else if (file.type.split("/")[0] !== "image") {
        alert("Файл не соответствует формату картинки");
        $(this).val("");
    }
});