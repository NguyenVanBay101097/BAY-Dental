import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsCareAfterOrderFormManualComponent } from './sms-care-after-order-form-manual.component';

describe('SmsCareAfterOrderFormManualComponent', () => {
  let component: SmsCareAfterOrderFormManualComponent;
  let fixture: ComponentFixture<SmsCareAfterOrderFormManualComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsCareAfterOrderFormManualComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsCareAfterOrderFormManualComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
