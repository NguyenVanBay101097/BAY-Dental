import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DateRangeAdvanceSearchComponent } from './date-range-advance-search.component';

describe('DateRangeAdvanceSearchComponent', () => {
  let component: DateRangeAdvanceSearchComponent;
  let fixture: ComponentFixture<DateRangeAdvanceSearchComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DateRangeAdvanceSearchComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DateRangeAdvanceSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
