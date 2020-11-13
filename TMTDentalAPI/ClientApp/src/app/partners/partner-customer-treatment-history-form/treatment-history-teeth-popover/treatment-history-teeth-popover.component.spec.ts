import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TreatmentHistoryTeethPopoverComponent } from './treatment-history-teeth-popover.component';

describe('TreatmentHistoryTeethPopoverComponent', () => {
  let component: TreatmentHistoryTeethPopoverComponent;
  let fixture: ComponentFixture<TreatmentHistoryTeethPopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TreatmentHistoryTeethPopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TreatmentHistoryTeethPopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
