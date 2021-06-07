import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsThanksFormAutomaticComponent } from './sms-thanks-form-automatic.component';

describe('SmsThanksFormAutomaticComponent', () => {
  let component: SmsThanksFormAutomaticComponent;
  let fixture: ComponentFixture<SmsThanksFormAutomaticComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsThanksFormAutomaticComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsThanksFormAutomaticComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
