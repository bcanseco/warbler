import 'bootstrap';



// ***************************************************************
//
//    Any scripts above will be loaded on all pages in the app.    
// ⚠️ If this file has "dist" in its name, DO NOT MAKE CHANGES ⚠️
//
// ***************************************************************
(function (i, s, o, g, r, a, m) {
  i["GoogleAnalyticsObject"] = r;
  i[r] = i[r] ||
    function () {
      (i[r].q = i[r].q || []).push(arguments);
    }, i[r].l = 1 * new Date();
  a = s.createElement(o),
    m = s.getElementsByTagName(o)[0];
  a.async = 1;
  a.src = g;
  m.parentNode.insertBefore(a, m);
})(window, document, "script", "https://www.google-analytics.com/analytics.js", "ga");

ga("create", "UA-92277807-1", "auto");
ga("send", "pageview");
