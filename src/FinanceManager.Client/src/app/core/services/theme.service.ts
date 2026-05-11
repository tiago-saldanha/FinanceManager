import { Injectable, signal } from '@angular/core';

export interface Theme {
  id: string;
  label: string;
  primary: string;
  revenue: string;
  expense: string;
}

export const THEMES: Theme[] = [
  { id: 'theme-teal',    label: 'Teal Moderno', primary: '#00695c', revenue: '#1D9E75', expense: '#D85A30' },
  { id: 'theme-emerald', label: 'Esmeralda',    primary: '#065f46', revenue: '#059669', expense: '#dc2626' },
  { id: 'theme-blue',    label: 'Azul',         primary: '#1565c0', revenue: '#1976d2', expense: '#D85A30' },
  { id: 'theme-purple',  label: 'Roxo',         primary: '#6a1b9a', revenue: '#1D9E75', expense: '#D85A30' },
];

const STORAGE_KEY = 'fm_theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly _current = signal<Theme>(this.loadTheme());

  readonly current = this._current.asReadonly();

  apply(theme: Theme): void {
    document.body.classList.remove(...THEMES.map(t => t.id));
    document.body.classList.add(theme.id);
    localStorage.setItem(STORAGE_KEY, theme.id);
    this._current.set(theme);
  }

  private loadTheme(): Theme {
    const saved = localStorage.getItem(STORAGE_KEY);
    const found = THEMES.find(t => t.id === saved);
    const theme = found ?? THEMES[0];
    document.body.classList.add(theme.id);
    return theme;
  }
}
