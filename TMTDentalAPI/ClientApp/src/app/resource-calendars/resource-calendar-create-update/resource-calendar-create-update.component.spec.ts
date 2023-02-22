import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceCalendarCreateUpdateComponent } from './resource-calendar-create-update.component';

describe('ResourceCalendarCreateUpdateComponent', () => {
  let component: ResourceCalendarCreateUpdateComponent;
  let fixture: ComponentFixture<ResourceCalendarCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResourceCalendarCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourceCalendarCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
