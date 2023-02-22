import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DateRangePickerFilterComponent } from './date-range-picker-filter.component';

describe('DateRangePickerFilterComponent', () => {
  let component: DateRangePickerFilterComponent;
  let fixture: ComponentFixture<DateRangePickerFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DateRangePickerFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DateRangePickerFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
