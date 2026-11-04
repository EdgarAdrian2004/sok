/* ==========================================================================
   SOK · App behaviour
   Theme, dialogs, toasts, navigation search and keyboard support.
   Classic script, no dependencies.
   ========================================================================== */

(function () {
  "use strict";

  var STORAGE_KEY = "sok-theme";
  var root = document.documentElement;

  /* ------------------------------------------------------------------------
     Theme
     ------------------------------------------------------------------------ */

  function readTheme() {
    try {
      return localStorage.getItem(STORAGE_KEY);
    } catch (error) {
      return null;
    }
  }

  function saveTheme(theme) {
    try {
      localStorage.setItem(STORAGE_KEY, theme);
    } catch (error) {
      /* Storage can be blocked on file:// in some browsers. */
    }
  }

  function currentTheme() {
    return root.dataset.theme === "light" ? "light" : "dark";
  }

  function applyTheme(theme, persist) {
    root.dataset.theme = theme;
    if (persist) saveTheme(theme);

    var meta = document.querySelector('meta[name="theme-color"]');
    if (meta) meta.setAttribute("content", theme === "dark" ? "#0B1120" : "#F8FAFC");

    Array.prototype.forEach.call(
      document.querySelectorAll("[data-theme-toggle]"),
      function (button) {
        var next = theme === "dark" ? "claro" : "oscuro";
        button.setAttribute("aria-label", "Cambiar a tema " + next);
        button.setAttribute("aria-pressed", theme === "dark" ? "true" : "false");
        button.setAttribute("data-tooltip", "Tema " + next);
      }
    );
  }

  function initTheme() {
    var stored = readTheme();
    var preferred = "dark";

    if (!stored) {
      try {
        preferred = window.matchMedia("(prefers-color-scheme: light)").matches
          ? "light"
          : "dark";
      } catch (error) {
        preferred = "dark";
      }
    }

    applyTheme(stored === "light" || stored === "dark" ? stored : preferred, false);
  }

  function bindTheme() {
    document.addEventListener("click", function (event) {
      var toggle = event.target.closest("[data-theme-toggle]");
      if (!toggle) return;
      applyTheme(currentTheme() === "dark" ? "light" : "dark", true);
    });
  }

  /* ------------------------------------------------------------------------
     Toasts
     ------------------------------------------------------------------------ */

  var TOAST_ICONS = {
    success: "check-circle",
    info: "info",
    warning: "alert-triangle",
    danger: "alert-circle"
  };

  function toast(options) {
    var settings = typeof options === "string" ? { text: options } : options || {};
    var tone = settings.tone || "success";
    var region = document.getElementById("toast-region");
    if (!region) return;

    var node = document.createElement("div");
    node.className = "toast toast--" + tone;
    node.setAttribute("role", tone === "danger" ? "alert" : "status");

    var iconName = TOAST_ICONS[tone] || "info";
    var iconMarkup = window.SOK && window.SOK.svg
      ? window.SOK.svg(iconName, "icon toast__icon")
      : "";

    node.innerHTML =
      iconMarkup +
      '<div class="toast__content">' +
      (settings.title
        ? '<div class="toast__title">' + settings.title + "</div>"
        : "") +
      '<div class="toast__text">' + (settings.text || "") + "</div>" +
      "</div>" +
      '<button class="toast__close" type="button" aria-label="Cerrar notificación">' +
      (window.SOK ? window.SOK.svg("x", "icon") : "×") +
      "</button>";

    region.appendChild(node);

    var timer = window.setTimeout(function () {
      dismiss();
    }, settings.duration || 6000);

    function dismiss() {
      window.clearTimeout(timer);
      if (node.parentNode) node.parentNode.removeChild(node);
    }

    node.querySelector(".toast__close").addEventListener("click", dismiss);
  }

  /* ------------------------------------------------------------------------
     Dialogs
     ------------------------------------------------------------------------ */

  var FOCUSABLE =
    'a[href], button:not([disabled]), input:not([disabled]), select:not([disabled]), ' +
    'textarea:not([disabled]), [tabindex]:not([tabindex="-1"])';

  var openStack = [];

  function focusables(container) {
    return Array.prototype.filter.call(
      container.querySelectorAll(FOCUSABLE),
      function (element) {
        return element.offsetParent !== null || element === document.activeElement;
      }
    );
  }

  function openDialog(id) {
    var dialog = document.getElementById(id);
    if (!dialog) return;

    var app = document.querySelector(".app");
    if (app) app.inert = true;

    dialog.hidden = false;
    dialog.setAttribute("aria-hidden", "false");

    var lastFocused = document.activeElement;
    openStack.push({ dialog: dialog, lastFocused: lastFocused });

    Array.prototype.forEach.call(
      dialog.querySelectorAll("[data-guard]"),
      function (checkbox) {
        checkbox.checked = false;
        syncGuard(checkbox);
      }
    );

    var target =
      dialog.querySelector("[data-autofocus]") ||
      focusables(dialog.querySelector(".dialog__panel") || dialog)[0] ||
      dialog;

    if (target === dialog) dialog.setAttribute("tabindex", "-1");
    window.setTimeout(function () {
      target.focus();
    }, 20);
  }

  function closeDialog(dialog) {
    if (!dialog) return;

    dialog.hidden = true;
    dialog.setAttribute("aria-hidden", "true");

    var index = openStack.findIndex(function (entry) {
      return entry.dialog === dialog;
    });

    var entry = index > -1 ? openStack.splice(index, 1)[0] : null;

    if (openStack.length === 0) {
      var app = document.querySelector(".app");
      if (app) app.inert = false;
    }

    if (entry && entry.lastFocused && entry.lastFocused.focus) {
      entry.lastFocused.focus();
    }
  }

  function topDialog() {
    return openStack.length ? openStack[openStack.length - 1].dialog : null;
  }

  function bindDialogs() {
    document.addEventListener("click", function (event) {
      var opener = event.target.closest("[data-dialog-open]");
      if (opener) {
        event.preventDefault();
        openDialog(opener.getAttribute("data-dialog-open"));
        return;
      }

      var closer = event.target.closest("[data-dialog-close]");
      if (closer) {
        event.preventDefault();
        closeDialog(closer.closest(".dialog"));
        return;
      }

      if (event.target.classList.contains("dialog__backdrop")) {
        closeDialog(event.target.closest(".dialog"));
      }
    });

    document.addEventListener("keydown", function (event) {
      var dialog = topDialog();
      if (!dialog) return;

      if (event.key === "Escape") {
        event.preventDefault();
        closeDialog(dialog);
        return;
      }

      if (event.key !== "Tab") return;

      var panel = dialog.querySelector(".dialog__panel") || dialog;
      var list = focusables(panel);
      if (list.length === 0) return;

      var first = list[0];
      var last = list[list.length - 1];

      if (event.shiftKey && document.activeElement === first) {
        event.preventDefault();
        last.focus();
      } else if (!event.shiftKey && document.activeElement === last) {
        event.preventDefault();
        first.focus();
      }
    });
  }

  /* ------------------------------------------------------------------------
     Shortcuts dialog (injected once)
     ------------------------------------------------------------------------ */

  function injectShortcutsDialog() {
    if (document.getElementById("dialog-shortcuts")) return;

    var rows = [
      ["Ctrl", "K", "Buscar una sección"],
      ["?", "", "Mostrar esta ayuda"],
      ["Esc", "", "Cerrar diálogo o limpiar búsqueda"],
      ["Tab", "", "Recorrer controles"],
      ["Enter", "", "Activar el control enfocado"]
    ];

    var markup =
      '<div class="dialog" id="dialog-shortcuts" role="dialog" aria-modal="true" ' +
      'aria-labelledby="dialog-shortcuts-title" aria-hidden="true" hidden>' +
      '<div class="dialog__backdrop"></div>' +
      '<div class="dialog__panel">' +
      '<div class="dialog__header">' +
      '<div class="dialog__heading">' +
      '<span class="dialog__icon" style="background:var(--color-accent-subtle);color:var(--color-accent-default)">' +
      (window.SOK ? window.SOK.svg("keyboard") : "") +
      "</span>" +
      "<div>" +
      '<h2 class="dialog__title" id="dialog-shortcuts-title">Atajos de teclado</h2>' +
      '<p class="dialog__subtitle">SOK se puede usar sin ratón</p>' +
      "</div></div>" +
      '<button class="btn btn--ghost btn--icon btn--sm" type="button" data-dialog-close aria-label="Cerrar">' +
      (window.SOK ? window.SOK.svg("x") : "×") +
      "</button></div>" +
      '<div class="dialog__body">' +
      '<div class="dialog__summary">' +
      rows
        .map(function (row) {
          return (
            '<div class="dialog__summary-row">' +
            "<dt>" + row[2] + "</dt>" +
            "<dd>" +
            '<span class="kbd">' + row[0] + "</span>" +
            (row[1] ? ' <span class="kbd">' + row[1] + "</span>" : "") +
            "</dd></div>"
          );
        })
        .join("") +
      "</div></div>" +
      '<div class="dialog__footer">' +
      '<button class="btn btn--primary" type="button" data-dialog-close data-autofocus>Entendido</button>' +
      "</div></div></div>";

    document.body.insertAdjacentHTML("beforeend", markup);
  }

  /* ------------------------------------------------------------------------
     Navigation search
     ------------------------------------------------------------------------ */

  function bindSearch() {
    var input = document.getElementById("nav-search");
    var nav = document.getElementById("primary-nav");
    if (!input || !nav) return;

    var empty = document.querySelector("[data-nav-empty]");

    function filter() {
      var query = input.value.trim().toLowerCase();
      var children = Array.prototype.slice.call(nav.children);
      var currentLabel = null;
      var groupItems = [];

      function flush() {
        if (!currentLabel) return;
        var hasMatch = groupItems.some(function (item) {
          return !item.hidden;
        });
        currentLabel.hidden = !hasMatch;
        currentLabel.style.display = hasMatch ? "" : "none";
      }

      children.forEach(function (child) {
        if (child.classList.contains("nav__label")) {
          flush();
          currentLabel = child;
          groupItems = [];
          return;
        }

        if (!child.hasAttribute("data-nav-item")) return;

        var label = child.getAttribute("data-label") || "";
        child.hidden = query !== "" && label.indexOf(query) === -1;
        groupItems.push(child);
      });

      flush();

      var anyVisible = Array.prototype.some.call(
        nav.querySelectorAll("[data-nav-item]"),
        function (item) {
          return !item.hidden;
        }
      );

      if (empty) empty.style.display = query !== "" && !anyVisible ? "" : "none";
    }

    input.addEventListener("input", filter);

    input.addEventListener("keydown", function (event) {
      if (event.key === "Escape") {
        input.value = "";
        filter();
        input.blur();
      }
    });
  }

  /* ------------------------------------------------------------------------
     Keyboard shortcuts
     ------------------------------------------------------------------------ */

  function isTyping(element) {
    if (!element) return false;
    var tag = element.tagName;
    return (
      tag === "INPUT" ||
      tag === "TEXTAREA" ||
      tag === "SELECT" ||
      element.isContentEditable
    );
  }

  function bindShortcuts() {
    document.addEventListener("keydown", function (event) {
      var meta = event.ctrlKey || event.metaKey;

      if (meta && event.key.toLowerCase() === "k") {
        event.preventDefault();
        var input = document.getElementById("nav-search");
        if (input) {
          input.focus();
          input.select();
        }
        return;
      }

      if (event.key === "?" && !isTyping(event.target)) {
        event.preventDefault();
        openDialog("dialog-shortcuts");
      }
    });
  }

  /* ------------------------------------------------------------------------
     Restore dialog data binding
     ------------------------------------------------------------------------ */

  function bindRestore() {
    document.addEventListener("click", function (event) {
      var trigger = event.target.closest("[data-restore-date]");
      if (!trigger) return;

      var values = {
        date: trigger.getAttribute("data-restore-date"),
        profile: trigger.getAttribute("data-restore-profile"),
        size: trigger.getAttribute("data-restore-size"),
        hash: trigger.getAttribute("data-restore-hash")
      };

      Object.keys(values).forEach(function (key) {
        var node = document.querySelector('[data-fill="' + key + '"]');
        if (node) node.textContent = values[key] || "—";
      });
    });
  }

  /* ------------------------------------------------------------------------
     Declarative demo actions
     ------------------------------------------------------------------------ */

  function syncGuard(checkbox) {
    var target = document.getElementById(checkbox.getAttribute("data-guard"));
    if (target) target.disabled = !checkbox.checked;
  }

  function bindGuards() {
    Array.prototype.forEach.call(
      document.querySelectorAll("[data-guard]"),
      syncGuard
    );

    document.addEventListener("change", function (event) {
      var checkbox = event.target.closest("[data-guard]");
      if (checkbox) syncGuard(checkbox);
    });
  }

  function bindDemoActions() {
    document.addEventListener("click", function (event) {
      var trigger = event.target.closest("[data-toast]");
      if (!trigger || trigger.tagName === "INPUT") return;

      toast({
        title: trigger.getAttribute("data-toast-title") || undefined,
        text: trigger.getAttribute("data-toast"),
        tone: trigger.getAttribute("data-toast-tone") || "success"
      });
    });

    document.addEventListener("change", function (event) {
      var input = event.target.closest('input[type="file"][data-file-toast]');
      if (!input || !input.files || !input.files.length) return;

      toast({
        title: "Archivo seleccionado",
        text:
          "«" + input.files[0].name + "» quedó listo para validar. La importación real llega con la versión WPF.",
        tone: "info"
      });

      input.value = "";
    });
  }

  /* ------------------------------------------------------------------------
     Boot
     ------------------------------------------------------------------------ */

  function boot() {
    initTheme();
    bindTheme();
    bindDialogs();
    bindDemoActions();
    bindGuards();
    bindRestore();
    bindSearch();
    bindShortcuts();
    injectShortcutsDialog();

    document.addEventListener("click", function (event) {
      var dismiss = event.target.closest("[data-dismiss]");
      if (!dismiss) return;
      var target = document.getElementById(dismiss.getAttribute("data-dismiss"));
      if (target) target.remove();
    });
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", boot);
  } else {
    boot();
  }

  window.SOK = window.SOK || {};
  window.SOK.toast = toast;
  window.SOK.openDialog = openDialog;
  window.SOK.closeDialog = closeDialog;
})();
