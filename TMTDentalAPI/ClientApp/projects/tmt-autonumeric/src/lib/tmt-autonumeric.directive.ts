import {
  AfterViewInit,
  Directive,
  ElementRef,
  EventEmitter,
  forwardRef,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  Renderer2,
  SimpleChanges,
} from '@angular/core';
import {ControlValueAccessor, NG_VALUE_ACCESSOR} from '@angular/forms';
import AutoNumeric from 'autonumeric';
import {AutonumericDefaults} from './tmt-autonumeric-defaults.service';

export const AUTONUMERIC_CONTROL_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => TmtAutonumericDirective),
  multi: true
};

@Directive({
  selector: '[tmtautonumeric]',
  providers: [AUTONUMERIC_CONTROL_VALUE_ACCESSOR],
  // tslint:disable-next-line:use-host-property-decorator
  host: {
    '(blur)': 'onTouchedFn()'
  },
})
export class TmtAutonumericDirective implements OnInit, AfterViewInit, OnChanges, OnDestroy, ControlValueAccessor {

  /* Defines the strategy to apply when options change.
   * reset will drop any previous options
   * update will change old values one by one but keep those that are not mentioned in the new options
   */
  @Input()
  strategy: 'reset' | 'update' = 'reset';

  @Input()
  options: any;
  @Input()
  predefined: string;
  instance: any;
  private isDisabled = false;
  private initialValue: any;
  unsubscribeFormat: () => void;
  unsubscribeRawValueModified: () => void;
  @Output()
  formatted = new EventEmitter();
  @Output()
  rawValueModified = new EventEmitter();
  onChangeFn: (value: any) => void = () => {
  };

  onTouchedFn = () => {
  };

  constructor(
    private elm: ElementRef,
    private defaults: AutonumericDefaults,
    private renderer: Renderer2,
  ) {
  }

  ngOnInit(): void {
  }

  private normalize(options: any) {
    const normalized = {};
    Object.keys(AutoNumeric.getDefaultConfig()).forEach(key => {
      if (typeof options[key] === 'undefined') {
        if (typeof (this.defaults as any)[key] !== 'undefined') {
          normalized[key] = (this.defaults as any)[key];
        }
      } else {
        normalized[key] = options[key];
      }
    });
    console.log(normalized);
    return normalized;
  }

  ngAfterViewInit(): void {
    var options = this.getOptions();
    console.log(options);

    this.instance = new AutoNumeric(
      this.elm.nativeElement,
      null,
      options
    );
    this.setDisabledState(this.isDisabled);
    this.unsubscribeFormat = this.renderer.listen(this.elm.nativeElement, 'autoNumeric:formatted', ($event) => {
      this.formatted.emit($event);
    });
    this.unsubscribeRawValueModified = this.renderer.listen(this.elm.nativeElement, 'autoNumeric:rawValueModified', ($event) => {
      // this.onChangeFn($event.detail.newRawValue);
      this.onChangeFn($event.detail.aNElement.getNumber());
      this.rawValueModified.emit($event);
    });
  }

  private getOptions(): AutoNumeric.Options {
    if (this.options === undefined && this.predefined === undefined) {
      return this.defaults;
    }
    if (this.options !== undefined && this.predefined !== undefined) {
      throw new Error('predefined attribute could not be combined with options. Please use either predefined or options');
    }
    if (this.options !== undefined) {
      return this.normalize(this.options);
    }
    const predefined = AutoNumeric.getPredefinedOptions()[this.predefined];
    return this.normalize(predefined);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.instance) {
      return;
    }
    if (changes.options || changes.predefined) {
      if (this.strategy === 'reset') {
        this.instance.options.reset();
      }
      this.instance.update(this.getOptions());
    }
  }

  ngOnDestroy(): void {
    this.unsubscribeFormat();
    this.unsubscribeRawValueModified();
    try {
      this.instance.remove(); // remove listeners
    } catch (e) {
    }
  }

  writeValue(value: any): void {
    console.log(value);
    console.log(this.instance);
    setTimeout(() => {
      if (this.instance) {
        this.instance.set(value);
      } else {
        // autonumeric hasn't been initialised yet, store the value for later use
        this.initialValue = value;
      }
    });

    // if (this.instance) {
    //   this.instance.set(value);
    // } else {
    //   // autonumeric hasn't been initialised yet, store the value for later use
    //   this.initialValue = value;
    // }
  }

  registerOnChange(fn: any): void {
    this.onChangeFn = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouchedFn = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.isDisabled = isDisabled;
    if (this.instance) {
      if (this.isDisabled) {
        this.renderer.setProperty(this.instance.domElement, 'disabled', 'disabled');
      } else {
        this.renderer.removeAttribute(this.instance.domElement, 'disabled');
      }
    }
  }
}