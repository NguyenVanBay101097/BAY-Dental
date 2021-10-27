import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardCardsMemberImportDialogComponent } from './card-cards-member-import-dialog.component';

describe('CardCardsMemberImportDialogComponent', () => {
  let component: CardCardsMemberImportDialogComponent;
  let fixture: ComponentFixture<CardCardsMemberImportDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardCardsMemberImportDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardCardsMemberImportDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
