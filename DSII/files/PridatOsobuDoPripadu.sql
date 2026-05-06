CREATE OR REPLACE PROCEDURE PridatOsobuDoPripadu_sp (
    p_cid   IN  NUMBER,
    p_pid   IN  NUMBER,
    p_roid  IN  NUMBER,
    p_autor IN  VARCHAR2,
    p_ret   OUT NUMBER
) AS
    v_stav      VARCHAR2(255);
    v_existuje  NUMBER;
    v_aktivni   NUMBER;
BEGIN
    p_ret := 0;

    BEGIN
        SELECT STAV INTO v_stav
        FROM PRIPAD
        WHERE PID = p_pid
        FOR UPDATE;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            ROLLBACK;
            RETURN;
    END;

    IF v_stav NOT IN ('open', 'running') THEN
        ROLLBACK;
        RETURN;
    END IF;

    SELECT COUNT(*) INTO v_existuje
    FROM CLOVEK_PRIPAD
    WHERE CID = p_cid AND PID = p_pid;

    IF v_existuje > 0 THEN
        ROLLBACK;
        RETURN;
    END IF;

    SELECT COUNT(*) INTO v_aktivni
    FROM CLOVEK_PRIPAD cp
    JOIN PRIPAD p ON p.PID = cp.PID
    WHERE cp.CID = p_cid
      AND p.STAV IN ('open', 'running');

    IF v_aktivni >= 10 THEN
        ROLLBACK;
        RETURN;
    END IF;

    INSERT INTO CLOVEK_PRIPAD (CID, PID, ROID)
    VALUES (p_cid, p_pid, p_roid);

    UPDATE CLOVEK
    SET POCET_PRIPADU              = POCET_PRIPADU + 1,
        POSLEDNI_AKTUALIZACE       = SYSDATE,
        AUTOR_POSLEDNI_AKTUALIZACE = p_autor
    WHERE CID = p_cid;

    COMMIT;
    p_ret := 1;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        p_ret := 0;
END;
/
