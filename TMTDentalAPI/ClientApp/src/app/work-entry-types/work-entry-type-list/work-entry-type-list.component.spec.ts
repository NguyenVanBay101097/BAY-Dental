import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkEntryTypeListComponent } from './work-entry-type-list.component';

describe('WorkEntryTypeListComponent', () => {
  let component: WorkEntryTypeListComponent;
  let fixture: ComponentFixture<WorkEntryTypeListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkEntryTypeListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkEntryTypeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
