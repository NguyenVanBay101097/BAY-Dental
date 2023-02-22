import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ToothSelectionDialogComponent } from './tooth-selection-dialog.component';

describe('ToothSelectionDialogComponent', () => {
  let component: ToothSelectionDialogComponent;
  let fixture: ComponentFixture<ToothSelectionDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ToothSelectionDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ToothSelectionDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
