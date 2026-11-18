// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("click", (event) => {
    const toggle = event.target.closest("[data-password-toggle]");
    if (!toggle) return;

    const wrapper = toggle.closest(".password-field-wrapper");
    const input = wrapper?.querySelector("input");
    if (!input) return;

    const isVisible = input.type === "text";
    input.type = isVisible ? "password" : "text";
    toggle.setAttribute("aria-pressed", String(!isVisible));
    toggle.setAttribute("aria-label", isVisible ? "Tampilkan kata sandi" : "Sembunyikan kata sandi");
    toggle.querySelector("[data-eye-visible]").hidden = !isVisible;
    toggle.querySelector("[data-eye-hidden]").hidden = isVisible;
});
