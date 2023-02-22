import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintPaperSizeListComponent } from './print-paper-size-list.component';

describe('PrintPaperSizeListComponent', () => {
  let component: PrintPaperSizeListComponent;
  let fixture: ComponentFixture<PrintPaperSizeListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintPaperSizeListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintPaperSizeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
