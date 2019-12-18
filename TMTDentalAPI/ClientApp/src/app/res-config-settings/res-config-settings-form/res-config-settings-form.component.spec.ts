import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResConfigSettingsFormComponent } from './res-config-settings-form.component';

describe('ResConfigSettingsFormComponent', () => {
  let component: ResConfigSettingsFormComponent;
  let fixture: ComponentFixture<ResConfigSettingsFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResConfigSettingsFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResConfigSettingsFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
