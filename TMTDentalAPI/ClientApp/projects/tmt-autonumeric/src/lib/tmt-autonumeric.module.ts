import { InjectionToken, ModuleWithProviders, NgModule } from '@angular/core';
import { AutonumericDefaults } from './tmt-autonumeric-defaults.service';
import { TmtAutonumericComponent } from './tmt-autonumeric.component';
import { TmtAutonumericDirective } from './tmt-autonumeric.directive';
import { AutonumericOptions } from './tmt-autonumeric.model';

export const USER_DEFAULTS = new InjectionToken('autonumeric defaults');

export function defaultsFactory(userDefaults: AutonumericOptions): AutonumericDefaults {
  const defaults: AutonumericDefaults = new AutonumericDefaults();
  Object.assign(defaults, userDefaults);
  return defaults;
}

@NgModule({
  declarations: [
    TmtAutonumericComponent,
    TmtAutonumericDirective
  ],
  imports: [
  ],
  exports: [
    TmtAutonumericComponent,
    TmtAutonumericDirective
  ]
})

export class TmtAutonumericModule {
  static forRoot(userDefaults: AutonumericOptions = {}): ModuleWithProviders {
      return {
          ngModule: TmtAutonumericModule,
          providers: [
              {
                  provide: USER_DEFAULTS,
                  useValue: userDefaults
              },
              {
                  provide: AutonumericDefaults,
                  useFactory: defaultsFactory,
                  deps: [USER_DEFAULTS]
              }
          ]
      };
  }
}
