// Apply styles when the document is ready
document.addEventListener('DOMContentLoaded', function () {
    // Check if we're on the login page
    if (window.location.pathname.includes('/Login')) {
        // Add class to body
        document.body.classList.add('login-page');

        // Find the login form container
        const formContainer = document.querySelector('.container form');
        if (formContainer) {
            // Wrap the form in our custom container
            const wrapper = document.createElement('div');
            wrapper.className = 'login-container';

            const header = document.createElement('div');
            header.className = 'login-header';
            header.innerHTML = '<h2>Log in</h2>';

            // Get the parent of the form
            const parent = formContainer.parentNode;

            // Replace with our wrapped version
            parent.insertBefore(wrapper, formContainer);
            wrapper.appendChild(header);
            wrapper.appendChild(formContainer);

            // Style the buttons
            const buttons = formContainer.querySelectorAll('.btn-primary');
            buttons.forEach(button => {
                button.style.backgroundColor = '#384B82';
                button.style.border = 'none';
                button.style.padding = '12px';
                button.style.borderRadius = '8px';
                button.style.fontWeight = '600';
                button.style.width = '100%';
            });

            // Style the inputs
            const inputs = formContainer.querySelectorAll('.form-control');
            inputs.forEach(input => {
                input.style.borderRadius = '8px';
                input.style.padding = '12px 15px';
                input.style.marginBottom = '15px';
            });
        }
    }
});
