//For Members page: Filter, MembershipType and Industries making it collapsible G.. Oi..

$(document).ready(function () {
    // Manual implementation of collapse functionality with transition effects
    $(".role-button").click(function (e) {
        e.preventDefault();

        // Get the target collapse element
        var targetId = $(this).data('bs-target');
        var target = $(targetId);

        // Get the arrow icon
        var arrowIcon;
        if (targetId.includes('collapseMembership')) {
            var itemId = targetId.replace('#collapseMembership', '');
            arrowIcon = $('#arrowMembership' + itemId);
        } else if (targetId.includes('collapseIndustry')) {
            var itemId = targetId.replace('#collapseIndustry', '');
            arrowIcon = $('#arrowIndustry' + itemId);
        }

        // Toggle the collapse with slide effect
        if (target.hasClass('show')) {
            // If it's open, close it with slideUp
            target.stop(true, true).slideUp(300, function () {
                target.removeClass('show');
            });
            if (arrowIcon) {
                arrowIcon.removeClass('bi-chevron-up').addClass('bi-chevron-down');
            }
        } else {
            // If it's closed, open it with slideDown
            target.stop(true, true).slideDown(300, function () {
                target.addClass('show');
            });
            if (arrowIcon) {
                arrowIcon.removeClass('bi-chevron-down').addClass('bi-chevron-up');
            }
        }
    });

    // Filter toggle functionality with transition
    $("#filterToggle").click(function () {
        var collapseFilter = $("#collapseFilter");

        if (collapseFilter.hasClass('show')) {
            collapseFilter.stop(true, true).slideUp(300, function () {
                collapseFilter.removeClass('show');
            });
            $(this).attr('aria-expanded', 'false');
        } else {
            collapseFilter.stop(true, true).slideDown(300, function () {
                collapseFilter.addClass('show');
            });
            $(this).attr('aria-expanded', 'true');
        }
    });

    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
});

// The Current toggle functions for the text sections
function toggleTextName(itemId) {
    var moreText = document.getElementById("moreTextName" + itemId);
    var readMore = document.getElementById("readMoreName" + itemId);

    if (moreText.style.display === "none") {
        moreText.style.display = "inline";
        readMore.innerHTML = " Read less";
    } else {
        moreText.style.display = "none";
        readMore.innerHTML = "Read More";
    }
}

function toggleTextCity(itemId) {
    var moreText = document.getElementById("moreTextCity" + itemId);
    var readMore = document.getElementById("readMoreCity" + itemId);

    if (moreText.style.display === "none") {
        moreText.style.display = "inline";
        readMore.innerHTML = " Read less";
    } else {
        moreText.style.display = "none";
        readMore.innerHTML = "Read More";
    }
}

function toggleTextIndustry(itemId) {
    var moreText = document.getElementById("moreTextIndustry" + itemId);
    var readMore = document.getElementById("readMoreIndustry" + itemId);

    if (moreText.style.display === "none") {
        moreText.style.display = "inline";
        readMore.innerHTML = " Read less";
    } else {
        moreText.style.display = "none";
        readMore.innerHTML = "Read More";
    }
}

function toggleIndustry(id) {
    var countSpan = document.getElementById("industryCount" + id);
    var collapseElement = document.getElementById("collapseIndustry" + id);

    if (collapseElement.classList.contains("show")) {
        countSpan.style.display = "inline";
    } else {
        countSpan.style.display = "none";
    }
}







// These scripts are for the Contacts page where Members and Opportunites are expandable and collapsible..


$(document).ready(function () {
    // Custom handler for Member Contacts and Opportunity Contacts collapsible sections
    $(".role-button").click(function (e) {
        e.preventDefault();

        // Get the target collapse element
        var targetId = $(this).data('bs-target');
        var target = $(targetId);

        // Get the arrow icon
        var arrowIcon;
        if (targetId.includes('collapseSpecalty')) {
            var itemId = targetId.replace('#collapseSpecalty', '');
            arrowIcon = $('#arrowSpec' + itemId);
        } else if (targetId.includes('collapseOpportunity')) {
            var itemId = targetId.replace('#collapseOpportunity', '');
            arrowIcon = $('#arrowOpportunity' + itemId);
        }

        // Toggle the collapse
        if (target.hasClass('show')) {
            // If it's open, close it
            target.removeClass('show');
            if (arrowIcon) {
                arrowIcon.removeClass('bi-chevron-up').addClass('bi-chevron-down');
            }
        } else {
            // If it's closed, open it
            target.addClass('show');
            if (arrowIcon) {
                arrowIcon.removeClass('bi-chevron-down').addClass('bi-chevron-up');
            }
        }
    });

    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });
});




// This is for the Opportunity page to make the Contacts expandable and Collapsible..


$(document).ready(function () {
    // Custom handler for Opportunity Contacts collapsible section
    $(".role-button").click(function (e) {
        e.preventDefault();

        // Get the target collapse element
        var targetId = $(this).data('bs-target');
        var target = $(targetId);

        // Get the arrow icon
        var arrowIcon;
        if (targetId.includes('collapseContact')) {
            var itemId = targetId.replace('#collapseContact', '');
            arrowIcon = $('#arrowContact' + itemId);
        }

        // Toggle the collapse
        if (target.hasClass('show')) {
            // If it's open, close it
            target.removeClass('show');
            if (arrowIcon) {
                arrowIcon.removeClass('bi-chevron-up').addClass('bi-chevron-down');
            }
        } else {
            // If it's closed, open it
            target.addClass('show');
            if (arrowIcon) {
                arrowIcon.removeClass('bi-chevron-down').addClass('bi-chevron-up');
            }
        }
    });
});







// This is for the industry Page to make the Subsectors ... to Read More..

function toggleTextSubsector(itemId) {
    var moreText = document.getElementById("moreTextSubsector" + itemId);
    var readMore = document.getElementById("readMoreSubsector" + itemId);

    // Toggle Subsector text
    if (moreText.style.display === "none") {
        moreText.style.display = "inline";
        readMore.innerHTML = " Read less";
    } else {
        moreText.style.display = "none";
        readMore.innerHTML = " Read More";
    }
}

function toggleTextSector(itemId) {
    var moreText = document.getElementById("moreTextSector" + itemId);
    var readMore = document.getElementById("readMoreSector" + itemId);

    // Toggle Sector text
    if (moreText.style.display === "none") {
        moreText.style.display = "inline";
        readMore.innerHTML = " Read less";
    } else {
        moreText.style.display = "none";
        readMore.innerHTML = " Read More";
    }
}








// This is for the Membership Types page to changes it into ReadMore instead of ... ..
function toggleTextType(itemId) {
    var moreText = document.getElementById("moreTextType" + itemId);
    var readMore = document.getElementById("readMoreType" + itemId);

    // Toggle membership type text
    if (moreText.style.display === "none") {
        moreText.style.display = "inline";
        readMore.innerHTML = " Read less";
    } else {
        moreText.style.display = "none";
        readMore.innerHTML = " Read More";
    }
}

function toggleTextDescription(itemId) {
    var moreText = document.getElementById("moreTextDescription" + itemId);
    var readMore = document.getElementById("readMoreDescription" + itemId);

    // Toggle Description text
    if (moreText.style.display === "none") {
        moreText.style.display = "inline";
        readMore.innerHTML = " Read less";
    } else {
        moreText.style.display = "none";
        readMore.innerHTML = " Read More";
    }
}

















































//document.addEventListener('DOMContentLoaded', function () {
//    // Handle filter toggle button using Bootstrap collapse
//    const filterToggle = document.getElementById('filterToggle');
//    const collapseFilter = document.getElementById('collapseFilter');

//    // This works with Bootstrap's collapse functionality
//    filterToggle.addEventListener('click', function () {
//        $(collapseFilter).collapse('toggle'); // Use Bootstrap's jQuery method to toggle
//    });

//    // Existing collapse handlers for specialty badges
//    document.querySelectorAll('[data-bs-toggle="collapse"]').forEach(function (element) {
//        var collapseId = element.getAttribute('href').replace('#', '');
//        var collapseElement = document.getElementById(collapseId);
//        var badgeElement = document.getElementById('badge' + collapseId.replace('collapseSpecialty', ''));

//        if (collapseElement && badgeElement) {
//            collapseElement.addEventListener('show.bs.collapse', function () {
//                badgeElement.style.display = 'none';
//            });

//            collapseElement.addEventListener('hide.bs.collapse', function () {
//                badgeElement.style.display = 'inline-block';
//            });
//        }
//    });
//});
