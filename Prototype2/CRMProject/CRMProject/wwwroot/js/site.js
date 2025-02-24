// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
    // Handle filter toggle button
    const filterToggle = document.getElementById('filterToggle');
    const collapseFilter = document.getElementById('collapseFilter');

    filterToggle.addEventListener('click', function () {
        const isExpanded = filterToggle.getAttribute('aria-expanded') === 'true';
        filterToggle.setAttribute('aria-expanded', !isExpanded);

        if (isExpanded) {
            collapseFilter.classList.remove('show');
        } else {
            collapseFilter.classList.add('show');
        }
    });

    // Existing collapse handlers for specialty badges
    document.querySelectorAll('[data-bs-toggle="collapse"]').forEach(function (element) {
        var collapseId = element.getAttribute('href').replace('#', '');
        var collapseElement = document.getElementById(collapseId);
        var badgeElement = document.getElementById('badge' + collapseId.replace('collapseSpecialty', ''));

        if (collapseElement && badgeElement) {
            collapseElement.addEventListener('show.bs.collapse', function () {
                badgeElement.style.display = 'none';
            });

            collapseElement.addEventListener('hide.bs.collapse', function () {
                badgeElement.style.display = 'inline-block';
            });
        }
    });
});
