/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "../SvelteComponents/dist/lexbox-svelte.js":
/*!*************************************************!*\
  !*** ../SvelteComponents/dist/lexbox-svelte.js ***!
  \*************************************************/
/***/ ((__unused_webpack___webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   DotNetServiceProvider: () => (/* binding */ ne),
/* harmony export */   DotNetServices: () => (/* binding */ te)
/* harmony export */ });
var Le = Object.defineProperty;
var Re = (e, t, n) => t in e ? Le(e, t, { enumerable: !0, configurable: !0, writable: !0, value: n }) : e[t] = n;
var $ = (e, t, n) => (Re(e, typeof t != "symbol" ? t + "" : t, n), n);
function g() {
}
function Se(e, t) {
  for (const n in t)
    e[n] = t[n];
  return (
    /** @type {T & S} */
    e
  );
}
function $e(e) {
  return e();
}
function oe() {
  return /* @__PURE__ */ Object.create(null);
}
function C(e) {
  e.forEach($e);
}
function ve(e) {
  return typeof e == "function";
}
function H(e, t) {
  return e != e ? t == t : e !== t || e && typeof e == "object" || typeof e == "function";
}
function Ie(e) {
  return Object.keys(e).length === 0;
}
function De(e, ...t) {
  if (e == null) {
    for (const r of t)
      r(void 0);
    return g;
  }
  const n = e.subscribe(...t);
  return n.unsubscribe ? () => n.unsubscribe() : n;
}
function xe(e, t, n) {
  e.$$.on_destroy.push(De(t, n));
}
function Ce(e, t, n, r) {
  if (e) {
    const s = we(e, t, n, r);
    return e[0](s);
  }
}
function we(e, t, n, r) {
  return e[1] && r ? Se(n.ctx.slice(), e[1](r(t))) : n.ctx;
}
function Me(e, t, n, r) {
  if (e[2] && r) {
    const s = e[2](r(n));
    if (t.dirty === void 0)
      return s;
    if (typeof s == "object") {
      const o = [], i = Math.max(t.dirty.length, s.length);
      for (let f = 0; f < i; f += 1)
        o[f] = t.dirty[f] | s[f];
      return o;
    }
    return t.dirty | s;
  }
  return t.dirty;
}
function Ue(e, t, n, r, s, o) {
  if (s) {
    const i = we(t, n, r, o);
    e.p(i, s);
  }
}
function Be(e) {
  if (e.ctx.length > 32) {
    const t = [], n = e.ctx.length / 32;
    for (let r = 0; r < n; r++)
      t[r] = -1;
    return t;
  }
  return -1;
}
function v(e, t) {
  e.appendChild(t);
}
function b(e, t, n) {
  e.insertBefore(t, n || null);
}
function E(e) {
  e.parentNode && e.parentNode.removeChild(e);
}
function ge(e, t) {
  for (let n = 0; n < e.length; n += 1)
    e[n] && e[n].d(t);
}
function y(e) {
  return document.createElement(e);
}
function w(e) {
  return document.createTextNode(e);
}
function me() {
  return w(" ");
}
function D(e, t, n, r) {
  return e.addEventListener(t, n, r), () => e.removeEventListener(t, n, r);
}
function S(e, t, n) {
  n == null ? e.removeAttribute(t) : e.getAttribute(t) !== n && e.setAttribute(t, n);
}
function Je(e) {
  return Array.from(e.childNodes);
}
function B(e, t) {
  t = "" + t, e.data !== t && (e.data = /** @type {string} */
  t);
}
function fe(e, t) {
  e.value = t ?? "";
}
function ye(e, t, n, r) {
  n == null ? e.style.removeProperty(t) : e.style.setProperty(t, n, r ? "important" : "");
}
function je(e) {
  const t = {};
  return e.childNodes.forEach(
    /** @param {Element} node */
    (n) => {
      t[n.slot || "default"] = !0;
    }
  ), t;
}
let Z;
function I(e) {
  Z = e;
}
const P = [], ue = [];
let R = [];
const le = [], Fe = /* @__PURE__ */ Promise.resolve();
let z = !1;
function He() {
  z || (z = !0, Fe.then(Ee));
}
function Q(e) {
  R.push(e);
}
const K = /* @__PURE__ */ new Set();
let N = 0;
function Ee() {
  if (N !== 0)
    return;
  const e = Z;
  do {
    try {
      for (; N < P.length; ) {
        const t = P[N];
        N++, I(t), Ke(t.$$);
      }
    } catch (t) {
      throw P.length = 0, N = 0, t;
    }
    for (I(null), P.length = 0, N = 0; ue.length; )
      ue.pop()();
    for (let t = 0; t < R.length; t += 1) {
      const n = R[t];
      K.has(n) || (K.add(n), n());
    }
    R.length = 0;
  } while (P.length);
  for (; le.length; )
    le.pop()();
  z = !1, K.clear(), I(e);
}
function Ke(e) {
  if (e.fragment !== null) {
    e.update(), C(e.before_update);
    const t = e.dirty;
    e.dirty = [-1], e.fragment && e.fragment.p(e.ctx, t), e.after_update.forEach(Q);
  }
}
function ze(e) {
  const t = [], n = [];
  R.forEach((r) => e.indexOf(r) === -1 ? t.push(r) : n.push(r)), n.forEach((r) => r()), R = t;
}
const M = /* @__PURE__ */ new Set();
let Qe;
function Oe(e, t) {
  e && e.i && (M.delete(e), e.i(t));
}
function Ye(e, t, n, r) {
  if (e && e.o) {
    if (M.has(e))
      return;
    M.add(e), Qe.c.push(() => {
      M.delete(e), r && (n && e.d(1), r());
    }), e.o(t);
  } else
    r && r();
}
function J(e) {
  return (e == null ? void 0 : e.length) !== void 0 ? e : Array.from(e);
}
function qe(e, t, n) {
  const { fragment: r, after_update: s } = e.$$;
  r && r.m(t, n), Q(() => {
    const o = e.$$.on_mount.map($e).filter(ve);
    e.$$.on_destroy ? e.$$.on_destroy.push(...o) : C(o), e.$$.on_mount = [];
  }), s.forEach(Q);
}
function Ge(e, t) {
  const n = e.$$;
  n.fragment !== null && (ze(n.after_update), C(n.on_destroy), n.fragment && n.fragment.d(t), n.on_destroy = n.fragment = null, n.ctx = []);
}
function We(e, t) {
  e.$$.dirty[0] === -1 && (P.push(e), He(), e.$$.dirty.fill(0)), e.$$.dirty[t / 31 | 0] |= 1 << t % 31;
}
function V(e, t, n, r, s, o, i = null, f = [-1]) {
  const c = Z;
  I(e);
  const u = e.$$ = {
    fragment: null,
    ctx: [],
    // state
    props: o,
    update: g,
    not_equal: s,
    bound: oe(),
    // lifecycle
    on_mount: [],
    on_destroy: [],
    on_disconnect: [],
    before_update: [],
    after_update: [],
    context: new Map(t.context || (c ? c.$$.context : [])),
    // everything else
    callbacks: oe(),
    dirty: f,
    skip_bound: !1,
    root: t.target || c.$$.root
  };
  i && i(u.root);
  let a = !1;
  if (u.ctx = n ? n(e, t.props || {}, (l, d, ...h) => {
    const m = h.length ? h[0] : d;
    return u.ctx && s(u.ctx[l], u.ctx[l] = m) && (!u.skip_bound && u.bound[l] && u.bound[l](m), a && We(e, l)), d;
  }) : [], u.update(), a = !0, C(u.before_update), u.fragment = r ? r(u.ctx) : !1, t.target) {
    if (t.hydrate) {
      const l = Je(t.target);
      u.fragment && u.fragment.l(l), l.forEach(E);
    } else
      u.fragment && u.fragment.c();
    t.intro && Oe(e.$$.fragment), qe(e, t.target, t.anchor), Ee();
  }
  I(c);
}
let Ae;
typeof HTMLElement == "function" && (Ae = class extends HTMLElement {
  constructor(t, n, r) {
    super();
    /** The Svelte component constructor */
    $(this, "$$ctor");
    /** Slots */
    $(this, "$$s");
    /** The Svelte component instance */
    $(this, "$$c");
    /** Whether or not the custom element is connected */
    $(this, "$$cn", !1);
    /** Component props data */
    $(this, "$$d", {});
    /** `true` if currently in the process of reflecting component props back to attributes */
    $(this, "$$r", !1);
    /** @type {Record<string, CustomElementPropDefinition>} Props definition (name, reflected, type etc) */
    $(this, "$$p_d", {});
    /** @type {Record<string, Function[]>} Event listeners */
    $(this, "$$l", {});
    /** @type {Map<Function, Function>} Event listener unsubscribe functions */
    $(this, "$$l_u", /* @__PURE__ */ new Map());
    this.$$ctor = t, this.$$s = n, r && this.attachShadow({ mode: "open" });
  }
  addEventListener(t, n, r) {
    if (this.$$l[t] = this.$$l[t] || [], this.$$l[t].push(n), this.$$c) {
      const s = this.$$c.$on(t, n);
      this.$$l_u.set(n, s);
    }
    super.addEventListener(t, n, r);
  }
  removeEventListener(t, n, r) {
    if (super.removeEventListener(t, n, r), this.$$c) {
      const s = this.$$l_u.get(n);
      s && (s(), this.$$l_u.delete(n));
    }
  }
  async connectedCallback() {
    if (this.$$cn = !0, !this.$$c) {
      let t = function(o) {
        return () => {
          let i;
          return {
            c: function() {
              i = y("slot"), o !== "default" && S(i, "name", o);
            },
            /**
             * @param {HTMLElement} target
             * @param {HTMLElement} [anchor]
             */
            m: function(u, a) {
              b(u, i, a);
            },
            d: function(u) {
              u && E(i);
            }
          };
        };
      };
      if (await Promise.resolve(), !this.$$cn)
        return;
      const n = {}, r = je(this);
      for (const o of this.$$s)
        o in r && (n[o] = [t(o)]);
      for (const o of this.attributes) {
        const i = this.$$g_p(o.name);
        i in this.$$d || (this.$$d[i] = U(i, o.value, this.$$p_d, "toProp"));
      }
      for (const o in this.$$p_d)
        !(o in this.$$d) && this[o] !== void 0 && (this.$$d[o] = this[o], delete this[o]);
      this.$$c = new this.$$ctor({
        target: this.shadowRoot || this,
        props: {
          ...this.$$d,
          $$slots: n,
          $$scope: {
            ctx: []
          }
        }
      });
      const s = () => {
        this.$$r = !0;
        for (const o in this.$$p_d)
          if (this.$$d[o] = this.$$c.$$.ctx[this.$$c.$$.props[o]], this.$$p_d[o].reflect) {
            const i = U(
              o,
              this.$$d[o],
              this.$$p_d,
              "toAttribute"
            );
            i == null ? this.removeAttribute(this.$$p_d[o].attribute || o) : this.setAttribute(this.$$p_d[o].attribute || o, i);
          }
        this.$$r = !1;
      };
      this.$$c.$$.after_update.push(s), s();
      for (const o in this.$$l)
        for (const i of this.$$l[o]) {
          const f = this.$$c.$on(o, i);
          this.$$l_u.set(i, f);
        }
      this.$$l = {};
    }
  }
  // We don't need this when working within Svelte code, but for compatibility of people using this outside of Svelte
  // and setting attributes through setAttribute etc, this is helpful
  attributeChangedCallback(t, n, r) {
    var s;
    this.$$r || (t = this.$$g_p(t), this.$$d[t] = U(t, r, this.$$p_d, "toProp"), (s = this.$$c) == null || s.$set({ [t]: this.$$d[t] }));
  }
  disconnectedCallback() {
    this.$$cn = !1, Promise.resolve().then(() => {
      this.$$cn || (this.$$c.$destroy(), this.$$c = void 0);
    });
  }
  $$g_p(t) {
    return Object.keys(this.$$p_d).find(
      (n) => this.$$p_d[n].attribute === t || !this.$$p_d[n].attribute && n.toLowerCase() === t
    ) || t;
  }
});
function U(e, t, n, r) {
  var o;
  const s = (o = n[e]) == null ? void 0 : o.type;
  if (t = s === "Boolean" && typeof t != "boolean" ? t != null : t, !r || !n[e])
    return t;
  if (r === "toAttribute")
    switch (s) {
      case "Object":
      case "Array":
        return t == null ? null : JSON.stringify(t);
      case "Boolean":
        return t ? "" : null;
      case "Number":
        return t ?? null;
      default:
        return t;
    }
  else
    switch (s) {
      case "Object":
      case "Array":
        return t && JSON.parse(t);
      case "Boolean":
        return t;
      case "Number":
        return t != null ? +t : t;
      default:
        return t;
    }
}
function k(e, t, n, r, s, o) {
  let i = class extends Ae {
    constructor() {
      super(e, n, s), this.$$p_d = t;
    }
    static get observedAttributes() {
      return Object.keys(t).map(
        (f) => (t[f].attribute || f).toLowerCase()
      );
    }
  };
  return Object.keys(t).forEach((f) => {
    Object.defineProperty(i.prototype, f, {
      get() {
        return this.$$c && f in this.$$c ? this.$$c[f] : this.$$d[f];
      },
      set(c) {
        var u;
        c = U(f, c, t), this.$$d[f] = c, (u = this.$$c) == null || u.$set({ [f]: c });
      }
    });
  }), r.forEach((f) => {
    Object.defineProperty(i.prototype, f, {
      get() {
        var c;
        return (c = this.$$c) == null ? void 0 : c[f];
      }
    });
  }), o && (i = o(i)), e.element = /** @type {any} */
  i, i;
}
class ee {
  constructor() {
    /**
     * ### PRIVATE API
     *
     * Do not use, may change at any time
     *
     * @type {any}
     */
    $(this, "$$");
    /**
     * ### PRIVATE API
     *
     * Do not use, may change at any time
     *
     * @type {any}
     */
    $(this, "$$set");
  }
  /** @returns {void} */
  $destroy() {
    Ge(this, 1), this.$destroy = g;
  }
  /**
   * @template {Extract<keyof Events, string>} K
   * @param {K} type
   * @param {((e: Events[K]) => void) | null | undefined} callback
   * @returns {() => void}
   */
  $on(t, n) {
    if (!ve(n))
      return g;
    const r = this.$$.callbacks[t] || (this.$$.callbacks[t] = []);
    return r.push(n), () => {
      const s = r.indexOf(n);
      s !== -1 && r.splice(s, 1);
    };
  }
  /**
   * @param {Partial<Props>} props
   * @returns {void}
   */
  $set(t) {
    this.$$set && !Ie(t) && (this.$$.skip_bound = !0, this.$$set(t), this.$$.skip_bound = !1);
  }
}
const Xe = "4";
typeof window < "u" && (window.__svelte || (window.__svelte = { v: /* @__PURE__ */ new Set() })).v.add(Xe);
function Ze(e) {
  let t, n, r, s, o, i, f, c;
  const u = (
    /*#slots*/
    e[3].default
  ), a = Ce(
    u,
    e,
    /*$$scope*/
    e[2],
    null
  );
  return {
    c() {
      t = y("button"), n = w("Svelte count is "), r = w(
        /*count*/
        e[0]
      ), s = w(`

  Slot content: "`), a && a.c(), o = w('"'), ye(t, "border", "solid pink 2px");
    },
    m(l, d) {
      b(l, t, d), v(t, n), v(t, r), v(t, s), a && a.m(t, null), v(t, o), i = !0, f || (c = D(
        t,
        "click",
        /*increment*/
        e[1]
      ), f = !0);
    },
    p(l, [d]) {
      (!i || d & /*count*/
      1) && B(
        r,
        /*count*/
        l[0]
      ), a && a.p && (!i || d & /*$$scope*/
      4) && Ue(
        a,
        u,
        l,
        /*$$scope*/
        l[2],
        i ? Me(
          u,
          /*$$scope*/
          l[2],
          d,
          null
        ) : Be(
          /*$$scope*/
          l[2]
        ),
        null
      );
    },
    i(l) {
      i || (Oe(a, l), i = !0);
    },
    o(l) {
      Ye(a, l), i = !1;
    },
    d(l) {
      l && E(t), a && a.d(l), f = !1, c();
    }
  };
}
function Ve(e, t, n) {
  let { $$slots: r = {}, $$scope: s } = t, o = 0;
  const i = () => {
    n(0, o += 1);
  };
  return e.$$set = (f) => {
    "$$scope" in f && n(2, s = f.$$scope);
  }, [o, i, s, r];
}
class ke extends ee {
  constructor(t) {
    super(), V(this, t, Ve, Ze, H, {});
  }
}
customElements.define("lexbox-counter", k(ke, {}, ["default"], [], !0));
function et(e) {
  let t, n, r, s, o;
  return {
    c() {
      t = y("button"), n = w("Svelte count is "), r = w(
        /*count*/
        e[0]
      ), ye(t, "border", "solid blue 2px");
    },
    m(i, f) {
      b(i, t, f), v(t, n), v(t, r), s || (o = D(
        t,
        "click",
        /*increment*/
        e[1]
      ), s = !0);
    },
    p(i, [f]) {
      f & /*count*/
      1 && B(
        r,
        /*count*/
        i[0]
      );
    },
    i: g,
    o: g,
    d(i) {
      i && E(t), s = !1, o();
    }
  };
}
function tt(e, t, n) {
  let r = 0;
  return [r, () => {
    n(0, r += 10);
  }];
}
class nt extends ee {
  constructor(t) {
    super(), V(this, t, tt, et, H, {});
  }
}
customElements.define("lexbox-counter2", k(nt, {}, [], [], !0));
var te = /* @__PURE__ */ ((e) => (e.LexboxApi = "LexboxApi", e))(te || {}), ce = Object.values(te);
class ne {
  static setService(t, n) {
    console.log("set-service"), this.validateServiceKey(t), this.services[t] = n;
  }
  static getService(t) {
    return this.validateServiceKey(t), this.services[t];
  }
  static validateServiceKey(t) {
    if (!ce.includes(t))
      throw new Error(`Invalid service key: ${t}. Valid vales are: ${ce.join(", ")}`);
  }
}
$(ne, "services", {});
window.lexbox = {
  DotNetServiceProvider: ne
};
const T = [];
function rt(e, t = g) {
  let n;
  const r = /* @__PURE__ */ new Set();
  function s(f) {
    if (H(e, f) && (e = f, n)) {
      const c = !T.length;
      for (const u of r)
        u[1](), T.push(u, e);
      if (c) {
        for (let u = 0; u < T.length; u += 2)
          T[u][0](T[u + 1]);
        T.length = 0;
      }
    }
  }
  function o(f) {
    s(f(e));
  }
  function i(f, c = g) {
    const u = [f, c];
    return r.add(u), r.size === 1 && (n = t(s, o) || g), f(e), () => {
      r.delete(u), r.size === 0 && n && (n(), n = null);
    };
  }
  return { set: s, update: o, subscribe: i };
}
/*!
 * https://github.com/Starcounter-Jack/JSON-Patch
 * (c) 2017-2022 Joachim Wester
 * MIT licensed
 */
var st = /* @__PURE__ */ function() {
  var e = function(t, n) {
    return e = Object.setPrototypeOf || { __proto__: [] } instanceof Array && function(r, s) {
      r.__proto__ = s;
    } || function(r, s) {
      for (var o in s)
        s.hasOwnProperty(o) && (r[o] = s[o]);
    }, e(t, n);
  };
  return function(t, n) {
    e(t, n);
    function r() {
      this.constructor = t;
    }
    t.prototype = n === null ? Object.create(n) : (r.prototype = n.prototype, new r());
  };
}(), it = Object.prototype.hasOwnProperty;
function Y(e, t) {
  return it.call(e, t);
}
function q(e) {
  if (Array.isArray(e)) {
    for (var t = new Array(e.length), n = 0; n < t.length; n++)
      t[n] = "" + n;
    return t;
  }
  if (Object.keys)
    return Object.keys(e);
  var r = [];
  for (var s in e)
    Y(e, s) && r.push(s);
  return r;
}
function _(e) {
  switch (typeof e) {
    case "object":
      return JSON.parse(JSON.stringify(e));
    case "undefined":
      return null;
    default:
      return e;
  }
}
function G(e) {
  for (var t = 0, n = e.length, r; t < n; ) {
    if (r = e.charCodeAt(t), r >= 48 && r <= 57) {
      t++;
      continue;
    }
    return !1;
  }
  return !0;
}
function O(e) {
  return e.indexOf("/") === -1 && e.indexOf("~") === -1 ? e : e.replace(/~/g, "~0").replace(/\//g, "~1");
}
function be(e) {
  return e.replace(/~1/g, "/").replace(/~0/g, "~");
}
function W(e) {
  if (e === void 0)
    return !0;
  if (e) {
    if (Array.isArray(e)) {
      for (var t = 0, n = e.length; t < n; t++)
        if (W(e[t]))
          return !0;
    } else if (typeof e == "object") {
      for (var r = q(e), s = r.length, o = 0; o < s; o++)
        if (W(e[r[o]]))
          return !0;
    }
  }
  return !1;
}
function ae(e, t) {
  var n = [e];
  for (var r in t) {
    var s = typeof t[r] == "object" ? JSON.stringify(t[r], null, 2) : t[r];
    typeof s < "u" && n.push(r + ": " + s);
  }
  return n.join(`
`);
}
var Ne = (
  /** @class */
  function(e) {
    st(t, e);
    function t(n, r, s, o, i) {
      var f = this.constructor, c = e.call(this, ae(n, { name: r, index: s, operation: o, tree: i })) || this;
      return c.name = r, c.index = s, c.operation = o, c.tree = i, Object.setPrototypeOf(c, f.prototype), c.message = ae(n, { name: r, index: s, operation: o, tree: i }), c;
    }
    return t;
  }(Error)
), p = Ne, ot = _, L = {
  add: function(e, t, n) {
    return e[t] = this.value, { newDocument: n };
  },
  remove: function(e, t, n) {
    var r = e[t];
    return delete e[t], { newDocument: n, removed: r };
  },
  replace: function(e, t, n) {
    var r = e[t];
    return e[t] = this.value, { newDocument: n, removed: r };
  },
  move: function(e, t, n) {
    var r = j(n, this.path);
    r && (r = _(r));
    var s = A(n, { op: "remove", path: this.from }).removed;
    return A(n, { op: "add", path: this.path, value: s }), { newDocument: n, removed: r };
  },
  copy: function(e, t, n) {
    var r = j(n, this.from);
    return A(n, { op: "add", path: this.path, value: _(r) }), { newDocument: n };
  },
  test: function(e, t, n) {
    return { newDocument: n, test: x(e[t], this.value) };
  },
  _get: function(e, t, n) {
    return this.value = e[t], { newDocument: n };
  }
}, ft = {
  add: function(e, t, n) {
    return G(t) ? e.splice(t, 0, this.value) : e[t] = this.value, { newDocument: n, index: t };
  },
  remove: function(e, t, n) {
    var r = e.splice(t, 1);
    return { newDocument: n, removed: r[0] };
  },
  replace: function(e, t, n) {
    var r = e[t];
    return e[t] = this.value, { newDocument: n, removed: r };
  },
  move: L.move,
  copy: L.copy,
  test: L.test,
  _get: L._get
};
function j(e, t) {
  if (t == "")
    return e;
  var n = { op: "_get", path: t };
  return A(e, n), n.value;
}
function A(e, t, n, r, s, o) {
  if (n === void 0 && (n = !1), r === void 0 && (r = !0), s === void 0 && (s = !0), o === void 0 && (o = 0), n && (typeof n == "function" ? n(t, 0, e, t.path) : F(t, 0)), t.path === "") {
    var i = { newDocument: e };
    if (t.op === "add")
      return i.newDocument = t.value, i;
    if (t.op === "replace")
      return i.newDocument = t.value, i.removed = e, i;
    if (t.op === "move" || t.op === "copy")
      return i.newDocument = j(e, t.from), t.op === "move" && (i.removed = e), i;
    if (t.op === "test") {
      if (i.test = x(e, t.value), i.test === !1)
        throw new p("Test operation failed", "TEST_OPERATION_FAILED", o, t, e);
      return i.newDocument = e, i;
    } else {
      if (t.op === "remove")
        return i.removed = e, i.newDocument = null, i;
      if (t.op === "_get")
        return t.value = e, i;
      if (n)
        throw new p("Operation `op` property is not one of operations defined in RFC-6902", "OPERATION_OP_INVALID", o, t, e);
      return i;
    }
  } else {
    r || (e = _(e));
    var f = t.path || "", c = f.split("/"), u = e, a = 1, l = c.length, d = void 0, h = void 0, m = void 0;
    for (typeof n == "function" ? m = n : m = F; ; ) {
      if (h = c[a], h && h.indexOf("~") != -1 && (h = be(h)), s && (h == "__proto__" || h == "prototype" && a > 0 && c[a - 1] == "constructor"))
        throw new TypeError("JSON-Patch: modifying `__proto__` or `constructor/prototype` prop is banned for security reasons, if this was on purpose, please set `banPrototypeModifications` flag false and pass it to this function. More info in fast-json-patch README");
      if (n && d === void 0 && (u[h] === void 0 ? d = c.slice(0, a).join("/") : a == l - 1 && (d = t.path), d !== void 0 && m(t, 0, e, d)), a++, Array.isArray(u)) {
        if (h === "-")
          h = u.length;
        else {
          if (n && !G(h))
            throw new p("Expected an unsigned base-10 integer value, making the new referenced value the array element with the zero-based index", "OPERATION_PATH_ILLEGAL_ARRAY_INDEX", o, t, e);
          G(h) && (h = ~~h);
        }
        if (a >= l) {
          if (n && t.op === "add" && h > u.length)
            throw new p("The specified index MUST NOT be greater than the number of elements in the array", "OPERATION_VALUE_OUT_OF_BOUNDS", o, t, e);
          var i = ft[t.op].call(t, u, h, e);
          if (i.test === !1)
            throw new p("Test operation failed", "TEST_OPERATION_FAILED", o, t, e);
          return i;
        }
      } else if (a >= l) {
        var i = L[t.op].call(t, u, h, e);
        if (i.test === !1)
          throw new p("Test operation failed", "TEST_OPERATION_FAILED", o, t, e);
        return i;
      }
      if (u = u[h], n && a < l && (!u || typeof u != "object"))
        throw new p("Cannot perform operation at the desired path", "OPERATION_PATH_UNRESOLVABLE", o, t, e);
    }
  }
}
function re(e, t, n, r, s) {
  if (r === void 0 && (r = !0), s === void 0 && (s = !0), n && !Array.isArray(t))
    throw new p("Patch sequence must be an array", "SEQUENCE_NOT_AN_ARRAY");
  r || (e = _(e));
  for (var o = new Array(t.length), i = 0, f = t.length; i < f; i++)
    o[i] = A(e, t[i], n, !0, s, i), e = o[i].newDocument;
  return o.newDocument = e, o;
}
function ut(e, t, n) {
  var r = A(e, t);
  if (r.test === !1)
    throw new p("Test operation failed", "TEST_OPERATION_FAILED", n, t, e);
  return r.newDocument;
}
function F(e, t, n, r) {
  if (typeof e != "object" || e === null || Array.isArray(e))
    throw new p("Operation is not an object", "OPERATION_NOT_AN_OBJECT", t, e, n);
  if (L[e.op]) {
    if (typeof e.path != "string")
      throw new p("Operation `path` property is not a string", "OPERATION_PATH_INVALID", t, e, n);
    if (e.path.indexOf("/") !== 0 && e.path.length > 0)
      throw new p('Operation `path` property must start with "/"', "OPERATION_PATH_INVALID", t, e, n);
    if ((e.op === "move" || e.op === "copy") && typeof e.from != "string")
      throw new p("Operation `from` property is not present (applicable in `move` and `copy` operations)", "OPERATION_FROM_REQUIRED", t, e, n);
    if ((e.op === "add" || e.op === "replace" || e.op === "test") && e.value === void 0)
      throw new p("Operation `value` property is not present (applicable in `add`, `replace` and `test` operations)", "OPERATION_VALUE_REQUIRED", t, e, n);
    if ((e.op === "add" || e.op === "replace" || e.op === "test") && W(e.value))
      throw new p("Operation `value` property is not present (applicable in `add`, `replace` and `test` operations)", "OPERATION_VALUE_CANNOT_CONTAIN_UNDEFINED", t, e, n);
    if (n) {
      if (e.op == "add") {
        var s = e.path.split("/").length, o = r.split("/").length;
        if (s !== o + 1 && s !== o)
          throw new p("Cannot perform an `add` operation at the desired path", "OPERATION_PATH_CANNOT_ADD", t, e, n);
      } else if (e.op === "replace" || e.op === "remove" || e.op === "_get") {
        if (e.path !== r)
          throw new p("Cannot perform the operation at a path that does not exist", "OPERATION_PATH_UNRESOLVABLE", t, e, n);
      } else if (e.op === "move" || e.op === "copy") {
        var i = { op: "_get", path: e.from, value: void 0 }, f = Te([i], n);
        if (f && f.name === "OPERATION_PATH_UNRESOLVABLE")
          throw new p("Cannot perform the operation from a path that does not exist", "OPERATION_FROM_UNRESOLVABLE", t, e, n);
      }
    }
  } else
    throw new p("Operation `op` property is not one of operations defined in RFC-6902", "OPERATION_OP_INVALID", t, e, n);
}
function Te(e, t, n) {
  try {
    if (!Array.isArray(e))
      throw new p("Patch sequence must be an array", "SEQUENCE_NOT_AN_ARRAY");
    if (t)
      re(_(t), _(e), n || !0);
    else {
      n = n || F;
      for (var r = 0; r < e.length; r++)
        n(e[r], r, t, void 0);
    }
  } catch (s) {
    if (s instanceof p)
      return s;
    throw s;
  }
}
function x(e, t) {
  if (e === t)
    return !0;
  if (e && t && typeof e == "object" && typeof t == "object") {
    var n = Array.isArray(e), r = Array.isArray(t), s, o, i;
    if (n && r) {
      if (o = e.length, o != t.length)
        return !1;
      for (s = o; s-- !== 0; )
        if (!x(e[s], t[s]))
          return !1;
      return !0;
    }
    if (n != r)
      return !1;
    var f = Object.keys(e);
    if (o = f.length, o !== Object.keys(t).length)
      return !1;
    for (s = o; s-- !== 0; )
      if (!t.hasOwnProperty(f[s]))
        return !1;
    for (s = o; s-- !== 0; )
      if (i = f[s], !x(e[i], t[i]))
        return !1;
    return !0;
  }
  return e !== e && t !== t;
}
const lt = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  JsonPatchError: p,
  _areEquals: x,
  applyOperation: A,
  applyPatch: re,
  applyReducer: ut,
  deepClone: ot,
  getValueByPointer: j,
  validate: Te,
  validator: F
}, Symbol.toStringTag, { value: "Module" }));
/*!
 * https://github.com/Starcounter-Jack/JSON-Patch
 * (c) 2017-2021 Joachim Wester
 * MIT license
 */
var se = /* @__PURE__ */ new WeakMap(), ct = (
  /** @class */
  /* @__PURE__ */ function() {
    function e(t) {
      this.observers = /* @__PURE__ */ new Map(), this.obj = t;
    }
    return e;
  }()
), at = (
  /** @class */
  /* @__PURE__ */ function() {
    function e(t, n) {
      this.callback = t, this.observer = n;
    }
    return e;
  }()
);
function ht(e) {
  return se.get(e);
}
function dt(e, t) {
  return e.observers.get(t);
}
function pt(e, t) {
  e.observers.delete(t.callback);
}
function _t(e, t) {
  t.unobserve();
}
function $t(e, t) {
  var n = [], r, s = ht(e);
  if (!s)
    s = new ct(e), se.set(e, s);
  else {
    var o = dt(s, t);
    r = o && o.observer;
  }
  if (r)
    return r;
  if (r = {}, s.value = _(e), t) {
    r.callback = t, r.next = null;
    var i = function() {
      X(r);
    }, f = function() {
      clearTimeout(r.next), r.next = setTimeout(i);
    };
    typeof window < "u" && (window.addEventListener("mouseup", f), window.addEventListener("keyup", f), window.addEventListener("mousedown", f), window.addEventListener("keydown", f), window.addEventListener("change", f));
  }
  return r.patches = n, r.object = e, r.unobserve = function() {
    X(r), clearTimeout(r.next), pt(s, r), typeof window < "u" && (window.removeEventListener("mouseup", f), window.removeEventListener("keyup", f), window.removeEventListener("mousedown", f), window.removeEventListener("keydown", f), window.removeEventListener("change", f));
  }, s.observers.set(t, new at(t, r)), r;
}
function X(e, t) {
  t === void 0 && (t = !1);
  var n = se.get(e.object);
  ie(n.value, e.object, e.patches, "", t), e.patches.length && re(n.value, e.patches);
  var r = e.patches;
  return r.length > 0 && (e.patches = [], e.callback && e.callback(r)), r;
}
function ie(e, t, n, r, s) {
  if (t !== e) {
    typeof t.toJSON == "function" && (t = t.toJSON());
    for (var o = q(t), i = q(e), f = !1, c = i.length - 1; c >= 0; c--) {
      var u = i[c], a = e[u];
      if (Y(t, u) && !(t[u] === void 0 && a !== void 0 && Array.isArray(t) === !1)) {
        var l = t[u];
        typeof a == "object" && a != null && typeof l == "object" && l != null && Array.isArray(a) === Array.isArray(l) ? ie(a, l, n, r + "/" + O(u), s) : a !== l && (s && n.push({ op: "test", path: r + "/" + O(u), value: _(a) }), n.push({ op: "replace", path: r + "/" + O(u), value: _(l) }));
      } else
        Array.isArray(e) === Array.isArray(t) ? (s && n.push({ op: "test", path: r + "/" + O(u), value: _(a) }), n.push({ op: "remove", path: r + "/" + O(u) }), f = !0) : (s && n.push({ op: "test", path: r, value: e }), n.push({ op: "replace", path: r, value: t }));
    }
    if (!(!f && o.length == i.length))
      for (var c = 0; c < o.length; c++) {
        var u = o[c];
        !Y(e, u) && t[u] !== void 0 && n.push({ op: "add", path: r + "/" + O(u), value: _(t[u]) });
      }
  }
}
function Pe(e, t, n) {
  n === void 0 && (n = !1);
  var r = [];
  return ie(e, t, r, "", n), r;
}
const vt = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  compare: Pe,
  generate: X,
  observe: $t,
  unobserve: _t
}, Symbol.toStringTag, { value: "Module" }));
Object.assign({}, lt, vt, {
  JsonPatchError: Ne,
  deepClone: _,
  escapePathComponent: O,
  unescapePathComponent: be
});
function he(e, t, n) {
  const r = e.slice();
  return r[8] = t[n], r[9] = t, r[10] = n, r;
}
function de(e, t, n) {
  const r = e.slice();
  return r[11] = t[n], r;
}
function pe(e) {
  let t, n, r = (
    /*sense*/
    e[11].gloss.values.en + ""
  ), s, o, i = (
    /*sense*/
    e[11].definition.values.en + ""
  ), f;
  return {
    c() {
      t = y("p"), n = w("- "), s = w(r), o = w(": "), f = w(i);
    },
    m(c, u) {
      b(c, t, u), v(t, n), v(t, s), v(t, o), v(t, f);
    },
    p(c, u) {
      u & /*$entries*/
      1 && r !== (r = /*sense*/
      c[11].gloss.values.en + "") && B(s, r), u & /*$entries*/
      1 && i !== (i = /*sense*/
      c[11].definition.values.en + "") && B(f, i);
    },
    d(c) {
      c && E(t);
    }
  };
}
function _e(e) {
  let t, n, r, s, o, i;
  function f() {
    e[4].call(
      n,
      /*each_value*/
      e[9],
      /*entry_index*/
      e[10]
    );
  }
  function c() {
    return (
      /*change_handler*/
      e[5](
        /*entry*/
        e[8]
      )
    );
  }
  let u = J(
    /*entry*/
    e[8].senses
  ), a = [];
  for (let l = 0; l < u.length; l += 1)
    a[l] = pe(de(e, u, l));
  return {
    c() {
      t = y("div"), n = y("input"), r = me(), s = y("div");
      for (let l = 0; l < a.length; l += 1)
        a[l].c();
      S(n, "type", "text"), S(s, "class", "sense"), S(t, "class", "entry");
    },
    m(l, d) {
      b(l, t, d), v(t, n), fe(
        n,
        /*entry*/
        e[8].lexemeForm.values.en
      ), v(t, r), v(t, s);
      for (let h = 0; h < a.length; h += 1)
        a[h] && a[h].m(s, null);
      o || (i = [
        D(n, "input", f),
        D(n, "change", c)
      ], o = !0);
    },
    p(l, d) {
      if (e = l, d & /*$entries*/
      1 && n.value !== /*entry*/
      e[8].lexemeForm.values.en && fe(
        n,
        /*entry*/
        e[8].lexemeForm.values.en
      ), d & /*$entries*/
      1) {
        u = J(
          /*entry*/
          e[8].senses
        );
        let h;
        for (h = 0; h < u.length; h += 1) {
          const m = de(e, u, h);
          a[h] ? a[h].p(m, d) : (a[h] = pe(m), a[h].c(), a[h].m(s, null));
        }
        for (; h < a.length; h += 1)
          a[h].d(1);
        a.length = u.length;
      }
    },
    d(l) {
      l && E(t), ge(a, l), o = !1, C(i);
    }
  };
}
function wt(e) {
  let t, n, r, s, o = J(
    /*$entries*/
    e[0]
  ), i = [];
  for (let f = 0; f < o.length; f += 1)
    i[f] = _e(he(e, o, f));
  return {
    c() {
      for (let f = 0; f < i.length; f += 1)
        i[f].c();
      t = me(), n = y("button"), n.textContent = "Refresh", S(n, "class", "btn btn-primary");
    },
    m(f, c) {
      for (let u = 0; u < i.length; u += 1)
        i[u] && i[u].m(f, c);
      b(f, t, c), b(f, n, c), r || (s = D(
        n,
        "click",
        /*refresh*/
        e[3]
      ), r = !0);
    },
    p(f, [c]) {
      if (c & /*$entries, updateEntry*/
      5) {
        o = J(
          /*$entries*/
          f[0]
        );
        let u;
        for (u = 0; u < o.length; u += 1) {
          const a = he(f, o, u);
          i[u] ? i[u].p(a, c) : (i[u] = _e(a), i[u].c(), i[u].m(t.parentNode, t));
        }
        for (; u < i.length; u += 1)
          i[u].d(1);
        i.length = o.length;
      }
    },
    i: g,
    o: g,
    d(f) {
      f && (E(t), E(n)), ge(i, f), r = !1, s();
    }
  };
}
function gt(e, t, n) {
  let r, s = rt([]);
  xe(e, s, (l) => n(0, r = l));
  let o;
  var i = ne.getService(te.LexboxApi);
  c();
  function f(l) {
    const d = o[l.id], h = Pe(d, l);
    console.log(h), i.invokeMethodAsync("UpdateEntry", l.id, h).then(c);
  }
  function c() {
    i.invokeMethodAsync("GetEntries", null).then((l) => {
      o = Object.fromEntries(l.map((d) => [d.id, d])), s.set(_(l));
    });
  }
  function u(l, d) {
    l[d].lexemeForm.values.en = this.value, s.set(r);
  }
  return [r, s, f, c, u, (l) => f(l)];
}
class mt extends ee {
  constructor(t) {
    super(), V(this, t, gt, wt, H, {});
  }
}
customElements.define("lexbox-main", k(mt, {}, [], [], !0));



/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		var cachedModule = __webpack_module_cache__[moduleId];
/******/ 		if (cachedModule !== undefined) {
/******/ 			return cachedModule.exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId](module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/************************************************************************/
/******/ 	/* webpack/runtime/define property getters */
/******/ 	(() => {
/******/ 		// define getter functions for harmony exports
/******/ 		__webpack_require__.d = (exports, definition) => {
/******/ 			for(var key in definition) {
/******/ 				if(__webpack_require__.o(definition, key) && !__webpack_require__.o(exports, key)) {
/******/ 					Object.defineProperty(exports, key, { enumerable: true, get: definition[key] });
/******/ 				}
/******/ 			}
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/hasOwnProperty shorthand */
/******/ 	(() => {
/******/ 		__webpack_require__.o = (obj, prop) => (Object.prototype.hasOwnProperty.call(obj, prop))
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/make namespace object */
/******/ 	(() => {
/******/ 		// define __esModule on exports
/******/ 		__webpack_require__.r = (exports) => {
/******/ 			if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 				Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 			}
/******/ 			Object.defineProperty(exports, '__esModule', { value: true });
/******/ 		};
/******/ 	})();
/******/ 	
/************************************************************************/
var __webpack_exports__ = {};
// This entry need to be wrapped in an IIFE because it need to be isolated against other modules in the chunk.
(() => {
var exports = __webpack_exports__;
/*!******************!*\
  !*** ./index.ts ***!
  \******************/

Object.defineProperty(exports, "__esModule", ({ value: true }));
__webpack_require__(/*! lexbox-svelte */ "../SvelteComponents/dist/lexbox-svelte.js");

})();

/******/ })()
;
//# sourceMappingURL=index.js.map