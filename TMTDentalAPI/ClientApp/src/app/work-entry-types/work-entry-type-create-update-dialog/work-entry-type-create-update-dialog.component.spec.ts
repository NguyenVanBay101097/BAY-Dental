import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkEntryTypeCreateUpdateDialogComponent } from './work-entry-type-create-update-dialog.component';

describe('WorkEntryTypeCreateUpdateDialogComponent', () => {
  let component: WorkEntryTypeCreateUpdateDialogComponent;
  let fixture: ComponentFixture<WorkEntryTypeCreateUpdateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkEntryTypeCreateUpdateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkEntryTypeCreateUpdateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
