$(document).ready(function () {

    $("#btnOpen").click(function () {
        $(".overlay,.popup").fadeIn();
        Call();
    })


    
    //function Call() {
    //    $.ajax({
    //        url: "",
    //        type: "post",
    //        contentType: "application/json",
    //        success: function () {
    //            setTimeout(function () { closeProggressBar() }, 10000);
    //        },
    //        error: function () {
    //            setTimeout(function () { }, 10000);

    //        }
    //    })
    //}
})