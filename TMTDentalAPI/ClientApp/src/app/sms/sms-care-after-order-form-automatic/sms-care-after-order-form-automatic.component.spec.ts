import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsCareAfterOrderFormAutomaticComponent } from './sms-care-after-order-form-automatic.component';

describe('SmsCareAfterOrderFormAutomaticComponent', () => {
  let component: SmsCareAfterOrderFormAutomaticComponent;
  let fixture: ComponentFixture<SmsCareAfterOrderFormAutomaticComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsCareAfterOrderFormAutomaticComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsCareAfterOrderFormAutomaticComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
