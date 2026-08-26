"use strict";

const demoButton = document.querySelector("[data-demo]");
const demoMessage = document.querySelector("[data-demo-message]");

if (demoButton && demoMessage) {
  demoButton.addEventListener("click", () => {
    demoButton.textContent = "Demo ready";
    demoButton.disabled = true;
    demoMessage.textContent = "This preview changes the interface only—no capture, files, or network activity occurs.";
  });
}
