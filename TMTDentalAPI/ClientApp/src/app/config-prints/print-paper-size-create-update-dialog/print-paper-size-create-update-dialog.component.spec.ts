import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintPaperSizeCreateUpdateDialogComponent } from './print-paper-size-create-update-dialog.component';

describe('PrintPaperSizeCreateUpdateDialogComponent', () => {
  let component: PrintPaperSizeCreateUpdateDialogComponent;
  let fixture: ComponentFixture<PrintPaperSizeCreateUpdateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintPaperSizeCreateUpdateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintPaperSizeCreateUpdateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
