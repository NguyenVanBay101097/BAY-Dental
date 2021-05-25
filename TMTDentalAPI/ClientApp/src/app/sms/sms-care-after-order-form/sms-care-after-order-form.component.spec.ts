import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsCareAfterOrderFormComponent } from './sms-care-after-order-form.component';

describe('SmsCareAfterOrderFormComponent', () => {
  let component: SmsCareAfterOrderFormComponent;
  let fixture: ComponentFixture<SmsCareAfterOrderFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsCareAfterOrderFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsCareAfterOrderFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
