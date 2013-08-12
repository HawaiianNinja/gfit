

// Bind to the click of the example link
$("a").click(function (event) {
  

    $.mobile.navigate("#bar", {
        info: "info about the #bar hash"
    });

    // Replace #bar with #baz
    $.mobile.navigate("#baz");


    // Log the results of the navigate event
    $(window).on("navigate", function (event, data) {
        console.log(data.state.info);
        console.log(data.state.direction);
        console.log(data.state.url);
        console.log(data.state.hash);
    });

    // Go back to pop the state for #bar and log it
    window.history.back();
});