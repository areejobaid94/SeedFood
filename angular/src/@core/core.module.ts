import { ModuleWithProviders, NgModule, Optional, SkipSelf } from '@angular/core';

import { CORE_CUSTOM_CONFIG } from '@core/services/config.service';

@NgModule()
export class CustomeCoreModule {
  constructor(@Optional() @SkipSelf() parentModule: CustomeCoreModule) {
    if (parentModule) {
      throw new Error('Import CoreModule in the AppModule only');
    }
  }

  static forRoot(config): ModuleWithProviders<CustomeCoreModule> {
    return {
      ngModule: CustomeCoreModule,
      providers: [
        {
          provide: CORE_CUSTOM_CONFIG,
          useValue: config
        }
      ]
    };
  }
}
