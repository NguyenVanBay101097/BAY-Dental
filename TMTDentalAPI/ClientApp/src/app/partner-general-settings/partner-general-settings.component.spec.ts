import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerGeneralSettingsComponent } from './partner-general-settings.component';

describe('PartnerGeneralSettingsComponent', () => {
  let component: PartnerGeneralSettingsComponent;
  let fixture: ComponentFixture<PartnerGeneralSettingsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerGeneralSettingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerGeneralSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
