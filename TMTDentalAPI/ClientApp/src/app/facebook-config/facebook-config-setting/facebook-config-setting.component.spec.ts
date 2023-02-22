import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookConfigSettingComponent } from './facebook-config-setting.component';

describe('FacebookConfigSettingComponent', () => {
  let component: FacebookConfigSettingComponent;
  let fixture: ComponentFixture<FacebookConfigSettingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookConfigSettingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookConfigSettingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
