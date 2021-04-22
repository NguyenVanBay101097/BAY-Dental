import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsTemplateCrUpComponent } from './sms-template-cr-up.component';

describe('SmsTemplateCrUpComponent', () => {
  let component: SmsTemplateCrUpComponent;
  let fixture: ComponentFixture<SmsTemplateCrUpComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsTemplateCrUpComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsTemplateCrUpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
