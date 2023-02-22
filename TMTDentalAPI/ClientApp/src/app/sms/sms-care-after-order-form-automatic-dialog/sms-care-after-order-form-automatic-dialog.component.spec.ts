import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsCareAfterOrderFormAutomaticDialogComponent } from './sms-care-after-order-form-automatic-dialog.component';

describe('SmsCareAfterOrderFormAutomaticDialogComponent', () => {
  let component: SmsCareAfterOrderFormAutomaticDialogComponent;
  let fixture: ComponentFixture<SmsCareAfterOrderFormAutomaticDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsCareAfterOrderFormAutomaticDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsCareAfterOrderFormAutomaticDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
