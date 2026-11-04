/* ==========================================================================
   SOK · Shell
   Injects the sidebar, header and icon sprite into every screen.
   Classic script (no modules) so the prototype works from file://.
   ========================================================================== */

(function () {
  "use strict";

  /* ------------------------------------------------------------------------
     Icon set · 24x24 outline, stroke = currentColor
     ------------------------------------------------------------------------ */

  var ICONS = {
    dashboard:
      '<rect x="3" y="3" width="7" height="7" rx="1.5"/><rect x="14" y="3" width="7" height="7" rx="1.5"/><rect x="3" y="14" width="7" height="7" rx="1.5"/><rect x="14" y="14" width="7" height="7" rx="1.5"/>',
    book:
      '<path d="M12 7v14"/><path d="M3 18a1 1 0 0 1-1-1V5a1 1 0 0 1 1-1h5a4 4 0 0 1 4 4 4 4 0 0 1 4-4h5a1 1 0 0 1 1 1v12a1 1 0 0 1-1 1h-6a3 3 0 0 0-3 3 3 3 0 0 0-3-3z"/>',
    users:
      '<path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M22 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/>',
    database:
      '<ellipse cx="12" cy="5" rx="9" ry="3"/><path d="M3 5v14a9 3 0 0 0 18 0V5"/><path d="M3 12a9 3 0 0 0 18 0"/>',
    activity: '<path d="M22 12h-4l-3 9L9 3l-3 9H2"/>',
    gear:
      '<circle cx="12" cy="12" r="3"/><path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 1 1-2.83 2.83l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 1 1-4 0v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 1 1-2.83-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 1 1 0-4h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 1 1 2.83-2.83l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 1 1 4 0v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 1 1 2.83 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 1 1 0 4h-.09a1.65 1.65 0 0 0-1.51 1z"/>',
    cpu:
      '<rect x="4" y="4" width="16" height="16" rx="2"/><rect x="9" y="9" width="6" height="6" rx="1"/><path d="M9 2v2M15 2v2M9 20v2M15 20v2M2 9h2M2 15h2M20 9h2M20 15h2"/>',
    drive:
      '<line x1="22" y1="12" x2="2" y2="12"/><path d="M5.45 5.11 2 12v6a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-6l-3.45-6.89A2 2 0 0 0 16.76 4H7.24a2 2 0 0 0-1.79 1.11z"/><line x1="6" y1="16" x2="6.01" y2="16"/><line x1="10" y1="16" x2="10.01" y2="16"/>',
    shield:
      '<path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/><path d="m9 12 2 2 4-4"/>',
    check: '<path d="M20 6 9 17l-5-5"/>',
    "check-circle": '<circle cx="12" cy="12" r="10"/><path d="m9 12 2 2 4-4"/>',
    "alert-triangle":
      '<path d="M10.29 3.86 1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/>',
    "alert-circle":
      '<circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/>',
    info:
      '<circle cx="12" cy="12" r="10"/><line x1="12" y1="16" x2="12" y2="12"/><line x1="12" y1="8" x2="12.01" y2="8"/>',
    x: '<path d="M18 6 6 18M6 6l12 12"/>',
    "chevron-right": '<path d="m9 18 6-6-6-6"/>',
    "chevron-left": '<path d="m15 18-6-6 6-6"/>',
    "chevron-down": '<path d="m6 9 6 6 6-6"/>',
    "arrow-right": '<path d="M5 12h14M12 5l7 7-7 7"/>',
    "arrow-left": '<path d="M19 12H5M12 19l-7-7 7-7"/>',
    sun:
      '<circle cx="12" cy="12" r="4"/><path d="M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M6.34 17.66l-1.41 1.41M19.07 4.93l-1.41 1.41"/>',
    moon: '<path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"/>',
    search: '<circle cx="11" cy="11" r="7"/><path d="m20 20-3.5-3.5"/>',
    help:
      '<circle cx="12" cy="12" r="10"/><path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"/><line x1="12" y1="17" x2="12.01" y2="17"/>',
    zap: '<path d="M13 2 3 14h9l-1 8 10-12h-9l1-8z"/>',
    clock: '<circle cx="12" cy="12" r="10"/><path d="M12 6v6l4 2"/>',
    refresh: '<path d="M21 12a9 9 0 1 1-2.64-6.36"/><path d="M21 3v6h-6"/>',
    download:
      '<path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><path d="m7 10 5 5 5-5"/><line x1="12" y1="15" x2="12" y2="3"/>',
    upload:
      '<path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><path d="m7 8 5-5 5 5"/><line x1="12" y1="3" x2="12" y2="15"/>',
    trash:
      '<path d="M3 6h18"/><path d="M8 6V4a1 1 0 0 1 1-1h6a1 1 0 0 1 1 1v2"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6"/><line x1="10" y1="11" x2="10" y2="17"/><line x1="14" y1="11" x2="14" y2="17"/>',
    play: '<path d="M6 4l14 8-14 8V4z"/>',
    rocket:
      '<path d="M4.5 16.5c-1.5 1.26-2 5-2 5s3.74-.5 5-2c.71-.84.7-2.13-.09-2.91a2.18 2.18 0 0 0-2.91-.09z"/><path d="M12 15l-3-3a22 22 0 0 1 2-3.95A12.88 12.88 0 0 1 22 2c0 2.72-.78 7.5-6 11a22.35 22.35 0 0 1-4 2z"/><path d="M9 12H4s.55-3.03 2-4c1.62-1.08 5 0 5 0"/><path d="M12 15v5s3.03-.55 4-2c1.08-1.62 0-5 0-5"/>',
    sparkles:
      '<path d="m12 3 1.9 5.1L19 10l-5.1 1.9L12 17l-1.9-5.1L5 10l5.1-1.9z"/><path d="M19 15l.9 2.1L22 18l-2.1.9L19 21l-.9-2.1L16 18l2.1-.9z"/>',
    monitor:
      '<rect x="2" y="3" width="20" height="14" rx="2"/><line x1="8" y1="21" x2="16" y2="21"/><line x1="12" y1="17" x2="12" y2="21"/>',
    gamepad:
      '<line x1="6" y1="12" x2="10" y2="12"/><line x1="8" y1="10" x2="8" y2="14"/><line x1="15" y1="13" x2="15.01" y2="13"/><line x1="18" y1="11" x2="18.01" y2="11"/><rect x="2" y="6" width="20" height="12" rx="4"/>',
    briefcase:
      '<rect x="2" y="7" width="20" height="14" rx="2"/><path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16"/>',
    lock:
      '<rect x="3" y="11" width="18" height="11" rx="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/>',
    file: '<path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><path d="M14 2v6h6"/><line x1="16" y1="13" x2="8" y2="13"/><line x1="16" y1="17" x2="8" y2="17"/>',
    more: '<circle cx="12" cy="12" r="1"/><circle cx="19" cy="12" r="1"/><circle cx="5" cy="12" r="1"/>',
    "rotate-ccw": '<path d="M3 12a9 9 0 1 0 3-6.7L3 8"/><path d="M3 3v5h5"/>',
    plus: '<path d="M12 5v14M5 12h14"/>',
    folder:
      '<path d="M4 20a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h4l2 3h8a2 2 0 0 1 2 2v9a2 2 0 0 1-2 2z"/>',
    calendar:
      '<rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/>',
    hash: '<line x1="4" y1="9" x2="20" y2="9"/><line x1="4" y1="15" x2="20" y2="15"/><line x1="10" y1="3" x2="8" y2="21"/><line x1="16" y1="3" x2="14" y2="21"/>',
    keyboard:
      '<rect x="2" y="6" width="20" height="12" rx="2"/><path d="M6 10h.01M10 10h.01M14 10h.01M18 10h.01M6 14h.01M18 14h.01M9 14h6"/>',
    eye: '<path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8S1 12 1 12z"/><circle cx="12" cy="12" r="3"/>',
    bell: '<path d="M18 8a6 6 0 0 0-12 0c0 7-3 9-3 9h18s-3-2-3-9"/><path d="M13.73 21a2 2 0 0 1-3.46 0"/>',
    external:
      '<path d="M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6"/><path d="M15 3h6v6"/><path d="M10 14 21 3"/>',
    gauge: '<path d="M12 14 8 9"/><path d="M3.34 19a10 10 0 1 1 17.32 0"/>',
    layers:
      '<path d="m12 2 9 5-9 5-9-5 9-5z"/><path d="m3 12 9 5 9-5"/><path d="m3 17 9 5 9-5"/>',
    wifi:
      '<path d="M5 12.55a11 11 0 0 1 14 0"/><path d="M1.42 9a16 16 0 0 1 21.16 0"/><path d="M8.53 16.11a6 6 0 0 1 6.95 0"/><line x1="12" y1="20" x2="12.01" y2="20"/>',
    user:
      '<path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/>',
    save: '<path d="M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z"/><path d="M17 21v-8H7v8"/><path d="M7 3v5h8"/>',
    filter: '<path d="M22 3H2l8 9.46V19l4 2v-8.54z"/>'
  };

  function svg(name, className) {
    var body = ICONS[name] || ICONS.help;
    return (
      '<svg class="' + (className || "icon") + '" viewBox="0 0 24 24" ' +
      'fill="none" stroke="currentColor" stroke-width="1.75" ' +
      'stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false">' +
      body +
      "</svg>"
    );
  }

  /* ------------------------------------------------------------------------
     Base path (works on file:// and http://)
     ------------------------------------------------------------------------ */

  var script =
    document.currentScript ||
    Array.prototype.slice
      .call(document.scripts)
      .filter(function (s) {
        return /shell\.js(\?|$)/.test(s.src);
      })
      .pop();

  var BASE = script ? script.src.replace(/shell\.js(\?.*)?$/, "") : "";

  /* ------------------------------------------------------------------------
     Navigation model
     ------------------------------------------------------------------------ */

  var NAV = [
    {
      label: "Operación",
      items: [
        { key: "dashboard", text: "Panel de control", icon: "dashboard", href: "screens/dashboard.html" },
        { key: "tutorial", text: "Tutorial", icon: "book", href: "screens/tutorial-welcome.html" },
        { key: "profiles", text: "Perfiles", icon: "users", href: "screens/profiles.html" },
        { key: "backups", text: "Respaldos", icon: "database", href: "screens/backups.html" }
      ]
    },
    {
      label: "Diagnóstico",
      items: [
        { key: "telemetry", text: "Telemetría", icon: "activity", pending: true },
        { key: "services", text: "Servicios", icon: "gear", pending: true },
        { key: "memory", text: "Memoria", icon: "cpu", pending: true }
      ]
    }
  ];

  var PAGE_ALIASES = {
    "tutorial-welcome": "tutorial",
    "tutorial-step02": "tutorial",
    "tutorial-final": "tutorial"
  };

  /* ------------------------------------------------------------------------
     Sidebar
     ------------------------------------------------------------------------ */

  function buildNav() {
    var page = document.body.dataset.page || "";
    var activeKey = PAGE_ALIASES[page] || page;

    return NAV.map(function (group) {
      var items = group.items
        .map(function (item) {
          var isActive = item.key === activeKey;
          var icon = svg(item.icon, "icon nav__icon");

          if (item.pending) {
            return (
              '<span class="nav__item nav__item--muted" aria-disabled="true" ' +
              'data-nav-item data-label="' + item.text.toLowerCase() + '">' +
              icon +
              '<span class="nav__text">' + item.text + "</span>" +
              '<span class="nav__badge">WPF</span></span>'
            );
          }

          return (
            '<a class="nav__item' + (isActive ? " nav__item--active" : "") + '" ' +
            'href="' + BASE + item.href + '"' +
            (isActive ? ' aria-current="page"' : "") +
            ' data-nav-item data-label="' + item.text.toLowerCase() + '">' +
            icon +
            '<span class="nav__text">' + item.text + "</span></a>"
          );
        })
        .join("");

      return (
        '<p class="nav__label">' + group.label + "</p>" +
        items
      );
    }).join("");
  }

  function sidebarHTML() {
    return (
      '<a class="sidebar__brand" href="' + BASE + 'index.html" aria-label="SOK, inicio del prototipo">' +
      '<span class="logo-mark">' + svg("shield") + "</span>" +
      '<span class="sidebar__brand-copy">' +
      '<span class="sidebar__brand-name">SOK</span>' +
      '<span class="sidebar__brand-tag">Optimización segura</span>' +
      "</span></a>" +
      '<div class="sidebar__scroll">' +
      '<nav class="nav" aria-label="Navegación principal" id="primary-nav">' +
      buildNav() +
      "</nav>" +
      '<p class="nav__label" style="display:none" data-nav-empty>Sin resultados</p>' +
      "</div>" +
      '<div class="sidebar__footer">' +
      '<div class="sidebar__device" title="Equipo de demostración">' +
      '<span class="sidebar__device-icon">' + svg("monitor") + "</span>" +
      '<span class="sidebar__device-copy">' +
      '<span class="sidebar__device-name">DESKTOP-NAVER</span>' +
      '<span class="sidebar__device-meta">Windows 11 · Ryzen 5</span>' +
      "</span></div>" +
      "</div>"
    );
  }

  /* ------------------------------------------------------------------------
     Header
     ------------------------------------------------------------------------ */

  function headerHTML() {
    var title = document.body.dataset.title || "SOK";
    var section = document.body.dataset.section || "Panel de control";

    return (
      '<div class="header__title">' +
      '<span class="header__eyebrow">SOK · ' + section + "</span>" +
      '<p class="header__heading">' + title + "</p>" +
      "</div>" +
      '<div class="header__actions">' +
      '<div class="header__search search" role="search">' +
      svg("search") +
      '<input class="search__input" id="nav-search" type="search" ' +
      'placeholder="Buscar sección…" aria-label="Buscar sección" autocomplete="off" ' +
      'aria-describedby="nav-search-hint">' +
      '<span class="visually-hidden" id="nav-search-hint">Filtra las secciones de la barra lateral. Atajo: Control más K.</span>' +
      "</div>" +
      '<span class="header__status"><span class="header__status-dot"></span>Protección activa</span>' +
      '<span class="header__divider" aria-hidden="true"></span>' +
      '<button class="btn btn--ghost btn--icon header__help" type="button" ' +
      'data-dialog-open="shortcuts" aria-label="Atajos de teclado" data-tooltip="Atajos (?)">' +
      svg("keyboard") + "</button>" +
      '<button class="theme-toggle" type="button" data-theme-toggle aria-pressed="false" ' +
      'aria-label="Cambiar a tema claro" data-tooltip="Cambiar tema">' +
      svg("moon", "icon theme-toggle__moon") +
      svg("sun", "icon theme-toggle__sun") +
      "</button>" +
      '<span class="avatar" aria-hidden="true">EA</span>' +
      "</div>"
    );
  }

  /* ------------------------------------------------------------------------
     Icon hydration for screen markup: <span data-icon="name"></span>
     ------------------------------------------------------------------------ */

  function hydrateIcons(root) {
    var nodes = (root || document).querySelectorAll("[data-icon]");
    Array.prototype.forEach.call(nodes, function (node) {
      var name = node.getAttribute("data-icon");
      var cls = node.getAttribute("class") || "icon";
      var holder = document.createElement("div");
      holder.innerHTML = svg(name, cls);
      node.replaceWith(holder.firstElementChild);
    });
  }

  /* ------------------------------------------------------------------------
     Mount
     ------------------------------------------------------------------------ */

  function mount() {
    var sidebar = document.getElementById("sidebar-mount");
    var header = document.getElementById("header-mount");

    if (sidebar) {
      sidebar.className = "sidebar";
      sidebar.setAttribute("aria-label", "Barra lateral");
      sidebar.innerHTML = sidebarHTML();
    }

    if (header) {
      header.className = "header";
      header.innerHTML = headerHTML();
    }

    hydrateIcons(document);

    if (!document.getElementById("toast-region")) {
      var region = document.createElement("div");
      region.className = "toast-region";
      region.id = "toast-region";
      region.setAttribute("role", "status");
      region.setAttribute("aria-live", "polite");
      region.setAttribute("aria-atomic", "false");
      document.body.appendChild(region);
    }
  }

  /* ------------------------------------------------------------------------
     Export
     ------------------------------------------------------------------------ */

  window.SOK = window.SOK || {};
  window.SOK.BASE = BASE;
  window.SOK.icons = ICONS;
  window.SOK.svg = svg;
  window.SOK.hydrateIcons = hydrateIcons;

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", mount);
  } else {
    mount();
  }
})();
