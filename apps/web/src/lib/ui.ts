/** Спільні UI-хелпери, щоб не дублювати їх по сторінках і формах. */

/** Число → рядок з одним знаком після коми; null → «—» (форма не ведеться). */
export const dash = (v: number | null) => (v == null ? "—" : v.toFixed(1));

/** Єдиний клас текстових інпутів (форми та пошук). */
export const inputCls =
  "w-full rounded-md border border-gray-300 px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500";
