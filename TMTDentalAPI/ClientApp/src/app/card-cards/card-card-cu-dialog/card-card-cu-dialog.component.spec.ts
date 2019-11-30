import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardCardCuDialogComponent } from './card-card-cu-dialog.component';

describe('CardCardCuDialogComponent', () => {
  let component: CardCardCuDialogComponent;
  let fixture: ComponentFixture<CardCardCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardCardCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardCardCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
