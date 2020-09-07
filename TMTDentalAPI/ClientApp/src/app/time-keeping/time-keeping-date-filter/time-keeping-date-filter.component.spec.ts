import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TimeKeepingDateFilterComponent } from './time-keeping-date-filter.component';

describe('TimeKeepingDateFilterComponent', () => {
  let component: TimeKeepingDateFilterComponent;
  let fixture: ComponentFixture<TimeKeepingDateFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TimeKeepingDateFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TimeKeepingDateFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
