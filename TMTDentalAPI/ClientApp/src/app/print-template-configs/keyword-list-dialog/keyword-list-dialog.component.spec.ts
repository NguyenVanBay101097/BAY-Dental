import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { KeywordListDialogComponent } from './keyword-list-dialog.component';

describe('KeywordListDialogComponent', () => {
  let component: KeywordListDialogComponent;
  let fixture: ComponentFixture<KeywordListDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ KeywordListDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(KeywordListDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
