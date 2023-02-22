import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DateRangePickerDropdownComponent } from './date-range-picker-dropdown.component';

describe('DateRangePickerDropdownComponent', () => {
  let component: DateRangePickerDropdownComponent;
  let fixture: ComponentFixture<DateRangePickerDropdownComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DateRangePickerDropdownComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DateRangePickerDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
