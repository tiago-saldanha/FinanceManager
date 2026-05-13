import { Directive, ElementRef, HostListener, inject } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';

@Directive({
  selector: 'input[appCurrencyMask]',
  standalone: true,
  providers: [
    { provide: NG_VALUE_ACCESSOR, useExisting: CurrencyMaskDirective, multi: true },
  ],
})
export class CurrencyMaskDirective implements ControlValueAccessor {
  private readonly el = inject(ElementRef<HTMLInputElement>);
  private onChange: (value: number | null) => void = () => {};
  private onTouched: () => void = () => {};

  @HostListener('input')
  onInput(): void {
    const raw = this.el.nativeElement.value;
    const digits = raw.replace(/\D/g, '');
    const numeric = digits.length ? parseInt(digits, 10) / 100 : null;
    this.el.nativeElement.value = numeric !== null ? this.format(numeric) : '';
    this.onChange(numeric);
  }

  @HostListener('blur')
  onBlur(): void {
    this.onTouched();
  }

  writeValue(value: number | null): void {
    this.el.nativeElement.value = value != null ? this.format(value) : '';
  }

  registerOnChange(fn: (value: number | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  private format(value: number): string {
    return value.toLocaleString('pt-BR', {
      style: 'currency',
      currency: 'BRL',
      minimumFractionDigits: 2,
    });
  }
}
