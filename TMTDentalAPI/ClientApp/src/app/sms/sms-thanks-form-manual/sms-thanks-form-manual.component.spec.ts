import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsThanksFormManualComponent } from './sms-thanks-form-manual.component';

describe('SmsThanksFormManualComponent', () => {
  let component: SmsThanksFormManualComponent;
  let fixture: ComponentFixture<SmsThanksFormManualComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsThanksFormManualComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsThanksFormManualComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
