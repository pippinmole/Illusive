
window.addEventListener("load", () => {
    // console.log(GetQueryValue("key1"))
});

function GetQueryValue(key) {
    const queryString = document.location.search.substr(1, document.location.search.length);
    const queries = queryString.split('&'); // [query, query, query]
    
    for(let i = 0; i < queries.length; i++) {
        const l = queries[i].split('=');
        if(l[0].toLowerCase() === key.toLowerCase()) {
            return l[1];
        }
    }
    
    return undefined;
}

function SetQueryValue(key, value) {
    const queryString = document.location.search.toLowerCase();
    const queryValue = GetQueryValue(key).toLowerCase();

    if(queryString === "" || queryString === undefined) {
        document.location.search += "?" + key + "=" + value;
    } else if(queryValue === undefined) {
        document.location.search += "&" + key + "=" + value;
    } else if(queryValue !== value) {
        document.location.search = document.location.search.replace(key + "=" + queryValue, key + "=" + value);
    }
}