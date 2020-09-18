import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceCalendarListComponent } from './resource-calendar-list.component';

describe('ResourceCalendarListComponent', () => {
  let component: ResourceCalendarListComponent;
  let fixture: ComponentFixture<ResourceCalendarListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResourceCalendarListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourceCalendarListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
